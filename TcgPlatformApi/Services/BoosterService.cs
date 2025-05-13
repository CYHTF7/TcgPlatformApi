using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;


namespace TcgPlatformApi.Services
{
    public class BoosterService : IBoosterService
    {
        private readonly AppDbContext _context;

        public BoosterService(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addboosters")]
        public async Task<bool> AddBoosterAsync(List<PlayerBoosterRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request.Quantity <= 0)
                {
                    throw new AppException(
                        userMessage: "Quantity must be more then 0",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[BoosterService] Quantity must be more then 0: {requests}"
                    );
                }

                var playerBooster = await _context.PlayerBoosters
                    .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.BoosterId == request.BoosterId);

                if (playerBooster == null)
                {
                    playerBooster = new Booster
                    {
                        PlayerId = request.PlayerId,
                        BoosterId = request.BoosterId,
                        Quantity = request.Quantity
                    };
                    _context.PlayerBoosters.Add(playerBooster);
                }
                else
                {
                    playerBooster.Quantity += request.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        [HttpGet("getallboosters")]
        public async Task<List<Booster>> GetPlayerBoostersAsync(int playerId)
        {
            if (playerId <= 0)
            {
                throw new AppException(
                    userMessage: "Invalid playerId",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[BoosterService] Invalid playerId: {playerId}"
                );
            }

            var playerBoosters = await _context.PlayerBoosters
                .Where(pb => pb.PlayerId == playerId)
                .ToListAsync();

            return (playerBoosters);
        }

        [HttpPost("removeboosters")]
        public async Task<bool> RemoveBoosterAsync(List<PlayerBoosterRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request.Quantity <= 0)
                {
                    throw new AppException(
                        userMessage: "Quantity must be more then 0",
                        statusCode: HttpStatusCode.BadRequest,
                        logMessage: $"[BoosterService] Quantity must be more then 0: {requests}"
                    );
                }

                var playerBooster = await _context.PlayerBoosters
                    .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.BoosterId == request.BoosterId);

                if (playerBooster == null)
                {
                    throw new AppException(
                        userMessage: "Booster not found for this player",
                        statusCode: HttpStatusCode.NotFound,
                        logMessage: $"[BoosterService] Booster not found for this player: {playerBooster}"
                    );
                }

                playerBooster.Quantity -= request.Quantity;

                if (playerBooster.Quantity <= 0)
                {
                    _context.PlayerBoosters.Remove(playerBooster);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
