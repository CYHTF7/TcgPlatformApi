using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface ICardService
    {
        Task<bool> AddCardsAsync(List<PlayerCardRequest> requests);
        Task<List<Card>> GetPlayerCardsAsync(int playerId);
        Task<bool> RemoveCardsAsync(List<PlayerCardRequest> requests);
    }
}
