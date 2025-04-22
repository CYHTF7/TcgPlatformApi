using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerCardService
    {
        Task<bool> AddCardsAsync(List<PlayerCardRequest> requests);
        Task<List<PlayerCard>> GetPlayerCardsAsync(int playerId);
        Task<bool> RemoveCardsAsync(List<PlayerCardRequest> requests);
    }
}
