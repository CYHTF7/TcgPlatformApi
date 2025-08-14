using Microsoft.EntityFrameworkCore;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class DeckCardService : IDeckCardService
    {
        private readonly AppDbContext _context;

        public DeckCardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCardInDeckAsync(DeckCardRequest request)
        {
            bool deckExists = await _context.PlayerDecks.AnyAsync(p => p.Id == request.DeckId);

            if (!deckExists)
            {
                throw new AppException(
                    userMessage: "Invalid DeckId",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Invalid DeckId: {request.DeckId}"
                );
            }

            if (request.Quantity <= 0)
            {
                throw new AppException(
                    userMessage: "Quantity must be more than 0",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Quantity must be more than 0: {request}"
                );
            }

            var existingDeckCard = await _context.PlayerDeckCards
                .FirstOrDefaultAsync(dc => dc.DeckId == request.DeckId && dc.CardId == request.CardId);

            if (existingDeckCard != null)
            {
                existingDeckCard.Quantity += request.Quantity;
            }
            else
            {
                var deckCard = new DeckCard
                {
                    DeckId = request.DeckId,
                    CardId = request.CardId,
                    Quantity = request.Quantity,
                    Order = request.Order
                };
                _context.PlayerDeckCards.Add(deckCard);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCardFromDeckAsync(DeckCardRemoveRequest request)
        {
            bool deckExists = await _context.PlayerDecks.AnyAsync(p => p.Id == request.DeckId);

            if (!deckExists)
            {
                throw new AppException(
                    userMessage: "Invalid DeckId",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Invalid DeckId: {request.DeckId}"
                );
            }

            if (request.Quantity <= 0)
            {
                throw new AppException(
                    userMessage: "Quantity must be more than 0",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Quantity must be more than 0: {request}"
                );
            }

            var existingDeckCard = await _context.PlayerDeckCards
                .FirstOrDefaultAsync(dc => dc.DeckId == request.DeckId && dc.CardId == request.CardId);

            if (existingDeckCard == null)
            {
                throw new AppException(
                    userMessage: "Card not found in deck",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[DeckCardService] Card not found in deck: DeckId={request.DeckId}, CardId={request.CardId}"
                );
            }

            if (existingDeckCard.Quantity < request.Quantity)
            {
                throw new AppException(
                    userMessage: $"Not enough cards to remove. Deck has {existingDeckCard.Quantity}, but requested to remove {request.Quantity}",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Not enough cards to remove: {existingDeckCard.Quantity} < {request.Quantity}"
                );
            }

            existingDeckCard.Quantity -= request.Quantity;

            if (existingDeckCard.Quantity <= 0)
            {
                _context.PlayerDeckCards.Remove(existingDeckCard);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCardsOrderAsync(List<DeckCardOrderRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                throw new AppException(
                    userMessage: "No cards to update",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Empty request list"
                );
            }

            var deckId = requests.First().DeckId;

            var deckExists = await _context.PlayerDecks.AnyAsync(p => p.Id == deckId);
            if (!deckExists)
            {
                throw new AppException(
                    userMessage: "Invalid DeckId",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[DeckCardService] Invalid DeckId: {deckId}"
                );
            }

            var deckCards = await _context.PlayerDeckCards
                .Where(dc => dc.DeckId == deckId)
                .ToListAsync();

            var cardsDict = deckCards.ToDictionary(c => c.CardId);

            foreach (var request in requests)
            {
                if (cardsDict.TryGetValue(request.CardId, out var card))
                {
                    card.Order = request.Order;
                }
                else
                {
                    throw new AppException(
                        userMessage: $"Card with id {request.CardId} not found in deck",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[DeckCardService] Card not found in deck: {request.CardId}"
                    );
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
