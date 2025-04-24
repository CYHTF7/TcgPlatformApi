using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class PlayerCardService : IPlayerCardService
    {
        private readonly AppDbContext _context;

        public PlayerCardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCardsAsync(List<PlayerCardRequest> requests) 
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

                if (request.Quantity <= 0)
                {
                    throw new AppException(
                        userMessage: "Quantity must be more then 0",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[CardService] Quantity must be more then 0: {requests}"
                    );
                }

                var playerCard = await _context.PlayerCards
                    .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.CardId == request.CardId);

                if (playerCard == null)
                {
                    playerCard = new PlayerCard
                    {
                        PlayerId = request.PlayerId,
                        CardId = request.CardId,
                        Quantity = request.Quantity
                    };
                    _context.PlayerCards.Add(playerCard);
                }
                else
                {
                    playerCard.Quantity += request.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PlayerCard>> GetPlayerCardsAsync(int playerId)
        {
            bool playerExists = await _context.PlayerProfiles.AnyAsync(p => p.Id == playerId);

            if (!playerExists)
            {
                throw new AppException(
                    userMessage: "Invalid playerId",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[CardService] Invalid playerId: {playerId}"
                );
            }

            var playerCards = await _context.PlayerCards
                .Where(pc => pc.PlayerId == playerId)
                .ToListAsync();

            return playerCards;
        }

        public async Task<bool> RemoveCardsAsync(List<PlayerCardRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request.Quantity <= 0)
                {
                    throw new AppException(
                        userMessage: "Quantity must be more then 0",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[CardService] Quantity must be more then 0: {requests}"
                    );
                }

                bool playerExists = await _context.PlayerProfiles.AnyAsync(p => p.Id == request.PlayerId);

                if (!playerExists)
                {
                    throw new AppException(
                        userMessage: "Invalid playerId",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[CardService] Invalid playerId: {request.PlayerId}"
                    );
                }

                var playerCard = await _context.PlayerCards
                    .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.CardId == request.CardId);

                if (playerCard == null)
                {
                    throw new AppException(
                        userMessage: "Invalid playerCard",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[CardService] Invalid playerCard: {playerCard}"
                    );
                }

                playerCard.Quantity -= request.Quantity;

                if (playerCard.Quantity <= 0)
                {
                    _context.PlayerCards.Remove(playerCard);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
