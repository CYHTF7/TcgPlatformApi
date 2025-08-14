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
                    Quantity = request.Quantity
                };
                _context.PlayerDeckCards.Add(deckCard);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCardFromDeckAsync(DeckCardRequest request)
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
    }
}
