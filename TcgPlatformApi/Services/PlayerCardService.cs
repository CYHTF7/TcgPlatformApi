using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
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
            try 
            {
                foreach (var request in requests)
                {
                    if (request.Quantity <= 0)
                    {
                        throw new ArgumentException("Quantity must be more then 0!");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }  

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PlayerCard>> GetPlayerCardsAsync(int playerId)
        {
            if (playerId <= 0)
            {
                throw new ArgumentException("Wrong player ID!");
            }

            var playerCards = await _context.PlayerCards
                .Where(pc => pc.PlayerId == playerId)
                .ToListAsync();

            return playerCards;
        }

        public async Task<bool> RemoveCardsAsync(List<PlayerCardRequest> requests)
        {
            try 
            {
                foreach (var request in requests)
                {
                    if (request.Quantity <= 0)
                    {
                        throw new ArgumentException("Quantity must be more then 0!");
                    }

                    var playerCard = await _context.PlayerCards
                        .FirstOrDefaultAsync(pc => pc.PlayerId == request.PlayerId && pc.CardId == request.CardId);

                    if (playerCard == null)
                    {
                        throw new ArgumentException("Invalid PlayerID!");
                    }

                    playerCard.Quantity -= request.Quantity;

                    if (playerCard.Quantity <= 0)
                    {
                        _context.PlayerCards.Remove(playerCard);
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
