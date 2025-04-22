using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
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

        public async Task<bool> AddDeckAsync(List<PlayerDeckRequest> requests)
        {
            foreach (var request in requests)
            {
                if (string.IsNullOrWhiteSpace(request.DeckName))
                {
                    throw new ArgumentException("Empty DeckName!");
                }

                PlayerDeck playerDeck;
                if (request.DeckId == 0)
                {
                    playerDeck = new PlayerDeck
                    {
                        DeckName = request.DeckName,
                        PlayerId = request.PlayerId
                    };
                    _context.PlayerDecks.Add(playerDeck);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    playerDeck = await _context.PlayerDecks
                        .FirstOrDefaultAsync(pd => pd.Id == request.DeckId && pd.PlayerId == request.PlayerId);

                    if (playerDeck == null)
                    {
                        throw new ArgumentNullException("Empty Deck!");
                    }
                    playerDeck.DeckName = request.DeckName;
                }

                int deckId = playerDeck.Id;

                var existingDeckCards = await _context.PlayerDeckCards
                    .Where(pdc => pdc.DeckId == deckId)
                    .ToListAsync();

                if (request.Cards == null || !request.Cards.Any())
                {
                    _context.PlayerDeckCards.RemoveRange(existingDeckCards);
                }
                else
                {
                    var existingDeckCardsDict = existingDeckCards.ToDictionary(pdc => pdc.CardId);

                    foreach (var card in request.Cards)
                    {
                        if (existingDeckCardsDict.TryGetValue(card.CardId, out var existingCard))
                        {
                            existingCard.Quantity = card.Quantity;
                        }
                        else
                        {
                            var newDeckCard = new PlayerDeckCard
                            {
                                DeckId = deckId,
                                CardId = card.CardId,
                                Quantity = card.Quantity
                            };
                            _context.PlayerDeckCards.Add(newDeckCard);
                        }
                    }

                    var requestCardIds = request.Cards.Select(c => c.CardId).ToHashSet();
                    var cardsToRemove = existingDeckCards
                        .Where(card => !requestCardIds.Contains(card.CardId))
                        .ToList();
                    _context.PlayerDeckCards.RemoveRange(cardsToRemove);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveDeckAsync(List<PlayerDeckRemoveRequest> requests)
        {
            foreach (var request in requests)
            {

                var deck = await _context.PlayerDecks.FirstOrDefaultAsync(d => d.Id == request.DeckId && d.PlayerId == request.PlayerId);

                if (deck == null)
                {
                    throw new InvalidOperationException($"Deck {request.DeckId} not found for player {request.PlayerId}");
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
            if (deckId <= 0)
            {
                throw new ArgumentException("Wrong deck ID!");
            }

            var deckWithCards = await _context.PlayerDecks
                .Where(d => d.Id == deckId)
                .Select(d => new PlayerDeckRequest
                {
                    DeckId = d.Id,
                    DeckName = d.DeckName,
                    PlayerId = d.PlayerId,
                    Cards = d.PlayerDeckCards.Select(c => new PlayerDeckCardRequest
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
            if (playerId <= 0)
            {
                throw new ArgumentException("Wrong player ID!");
            }

            var decks = await _context.PlayerDecks
                .Where(d => d.PlayerId == playerId)
                .Select(d => new PlayerDeckRequest
                {
                    DeckId = d.Id,
                    DeckName = d.DeckName,
                    PlayerId = d.PlayerId,
                    Cards = d.PlayerDeckCards.Select(c => new PlayerDeckCardRequest
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
