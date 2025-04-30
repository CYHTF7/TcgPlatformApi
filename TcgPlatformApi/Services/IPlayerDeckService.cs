using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerDeckService
    {
        Task<bool> AddorUpdateDecksAsync(List<PlayerDeckRequest> requests);
        Task<bool> RemoveDeckAsync(List<PlayerDeckRemoveRequest> request);
        Task<PlayerDeckRequest> GetDeckAsync(int deckId);
        Task<List<PlayerDeckRequest>> GetDecksByPlayerIdAsync(int playerId);
    }
}
