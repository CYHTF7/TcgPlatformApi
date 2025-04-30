using Microsoft.EntityFrameworkCore;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class PlayerDeckService : IPlayerDeckService
    {
        private readonly AppDbContext _context;

        public PlayerDeckService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddorUpdateDecksAsync(List<PlayerDeckRequest> requests)
        {
            foreach (var request in requests)
            {
                bool playerExists = await _context.PlayerProfiles.AnyAsync(p => p.Id == request.PlayerId);

                if (!playerExists)
                {
                    throw new AppException(
                        userMessage: "Invalid playerId",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[CardService] Invalid playerId: {request.PlayerId}"
                    );
                }

                if (string.IsNullOrWhiteSpace(request.DeckName))
                {
                    throw new AppException(
                        userMessage: "DeckName must be filled",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[DeckService] DeckName must be filled: {request.PlayerId}"
                    );
                }

                Deck playerDeck;
                if (request.DeckId == 0)
                {
                    playerDeck = new Deck
                    {
                        DeckName = request.DeckName,
                        PlayerId = request.PlayerId
                    };

                    _context.PlayerDecks.Add(playerDeck);

                    await _context.SaveChangesAsync();

                    await SyncDeckCardsAsync(playerDeck.Id, request.Cards);
                }
                else
                {
                    await UpdateDeckAsync(request);
                }     
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task UpdateDeckAsync(PlayerDeckRequest request) 
        {
            var playerDeck = await _context.PlayerDecks
                .FirstOrDefaultAsync(pd => pd.Id == request.DeckId && pd.PlayerId == request.PlayerId);

            if (playerDeck == null)
            {
                throw new AppException(
                    userMessage: "Invalid playerDeck",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckService] Invalid playerDeck: {request.DeckId}"
                );
            }

            playerDeck.DeckName = request.DeckName;

            await SyncDeckCardsAsync(playerDeck.Id, request.Cards);
        }

        private async Task SyncDeckCardsAsync(int deckId, List<PlayerDeckCardDTO> cards) 
        {
            cards = cards?.Where(c => c.CardId > 0 && c.Quantity > 0)
                .ToList() ?? new List<PlayerDeckCardDTO>();

            var existingDeckCards = await _context.PlayerDeckCards
                .Where(pdc => pdc.DeckId == deckId)
                .ToListAsync();

            if (cards == null || !cards.Any())
            {
                _context.PlayerDeckCards.RemoveRange(existingDeckCards);
                return;
            }

            var existingDeckCardsDict = existingDeckCards.ToDictionary(pdc => pdc.CardId);

            foreach (var card in cards)
            {
                if (existingDeckCardsDict.TryGetValue(card.CardId, out var existingCard))
                {
                    existingCard.Quantity = card.Quantity;
                }
                else
                {
                    var newDeckCard = new DeckCard
                    {
                        DeckId = deckId,
                        CardId = card.CardId,
                        Quantity = card.Quantity
                    };
                    _context.PlayerDeckCards.Add(newDeckCard);
                }
            }

            var requestCardIds = cards.Select(c => c.CardId).ToHashSet();
            var cardsToRemove = existingDeckCards
                .Where(card => !requestCardIds.Contains(card.CardId))
                .ToList();

            _context.PlayerDeckCards.RemoveRange(cardsToRemove);
        }

        public async Task<bool> RemoveDeckAsync(List<PlayerDeckRemoveRequest> requests)
        {
            foreach (var request in requests)
            {
                bool playerExists = await _context.PlayerProfiles.AnyAsync(p => p.Id == request.PlayerId);

                if (!playerExists)
                {
                    throw new AppException(
                        userMessage: "Invalid playerId",
                        statusCode: HttpStatusCode.NotFound,
                        logMessage: $"[CardService] Invalid playerId: {request.PlayerId}"
                    );
                }

                var deck = await _context.PlayerDecks.FirstOrDefaultAsync(d => d.Id == request.DeckId && d.PlayerId == request.PlayerId);

                bool deckExists = await _context.PlayerDecks.AnyAsync(d => d.Id == request.DeckId);

                if (!deckExists)
                {
                    throw new AppException(
                        userMessage: "Invalid deckId",
                        statusCode: HttpStatusCode.NotFound,
                        logMessage: $"[DeckService] Invalid deckId: {deck.Id}"
                    );
                }

                var cardsToRemove = await _context.PlayerDeckCards.Where(c => c.DeckId == request.DeckId).ToListAsync();

                if (cardsToRemove.Any())
                {
                    _context.PlayerDeckCards.RemoveRange(cardsToRemove);
                }

                _context.PlayerDecks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlayerDeckRequest> GetDeckAsync(int deckId)
        {
            bool deckExists = await _context.PlayerDecks.AnyAsync(d => d.Id == deckId);

            if (!deckExists)
            {
                throw new AppException(
                    userMessage: "Invalid deckId",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[DeckService] Invalid deckId: {deckId}"
                );
            }

            var deckWithCards = await _context.PlayerDecks
                .Where(d => d.Id == deckId)
                .Select(d => new PlayerDeckRequest
                {
                    DeckId = d.Id,
                    DeckName = d.DeckName,
                    PlayerId = d.PlayerId,
                    Cards = d.PlayerDeckCards.Select(c => new PlayerDeckCardDTO
                    {
                        CardId = c.CardId,
                        Quantity = c.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return deckWithCards;
        }

        public async Task<List<PlayerDeckRequest>> GetDecksByPlayerIdAsync(int playerId)
        {
            bool playerExists = await _context.PlayerProfiles.AnyAsync(p => p.Id == playerId);

            if (!playerExists)
            {
                throw new AppException(
                    userMessage: "Invalid playerId",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[CardService] Invalid playerId: {playerId}"
                );
            }

            var decks = await _context.PlayerDecks
                .Where(d => d.PlayerId == playerId)
                .Select(d => new PlayerDeckRequest
                {
                    DeckId = d.Id,
                    DeckName = d.DeckName,
                    PlayerId = d.PlayerId,
                    Cards = d.PlayerDeckCards.Select(c => new PlayerDeckCardDTO
                    {
                        CardId = c.CardId,
                        Quantity = c.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return decks;
        }
    }
}
