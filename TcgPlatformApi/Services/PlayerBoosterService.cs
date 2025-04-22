using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class PlayerBoosterService : IPlayerBoosterService
    {
        private readonly AppDbContext _context;

        public PlayerBoosterService(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addboosters")]
        public async Task<bool> AddBoosterAsync(List<PlayerBoosterRequest> requests)
        {
            try 
            {
                foreach (var request in requests)
                {
                    if (request.Quantity <= 0)
                    {
                        throw new ArgumentException("Quantity must be more then 0!");
                    }

                    var playerBooster = await _context.PlayerBoosters
                        .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.BoosterId == request.BoosterId);

                    if (playerBooster == null)
                    {
                        playerBooster = new PlayerBooster
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        [HttpGet("getallboosters")]
        public async Task<List<PlayerBooster>> GetPlayerBoostersAsync(int playerId)
        {
            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID!");
            }

            var playerBoosters = await _context.PlayerBoosters
                .Where(pb => pb.PlayerId == playerId)
                .ToListAsync();

            return (playerBoosters);
        }

        [HttpPost("removeboosters")]
        public async Task<bool> RemoveBoosterAsync(List<PlayerBoosterRequest> requests)
        {
            try 
            {
                foreach (var request in requests)
                {
                    if (request.Quantity <= 0)
                    {
                        throw new ArgumentException("Quantity must be more then 0!");
                    }

                    var playerBooster = await _context.PlayerBoosters
                        .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.BoosterId == request.BoosterId);

                    if (playerBooster == null)
                    {
                        throw new ArgumentException("Booster not found for this player!");
                    }

                    playerBooster.Quantity -= request.Quantity;

                    if (playerBooster.Quantity <= 0)
                    {
                        _context.PlayerBoosters.Remove(playerBooster);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
