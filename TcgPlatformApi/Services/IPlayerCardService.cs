using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerCardService
    {
        Task<bool> AddCardsAsync(List<PlayerCardRequest> requests);
        Task<List<Card>> GetPlayerCardsAsync(int playerId);
        Task<bool> RemoveCardsAsync(List<PlayerCardRequest> requests);
    }
}
