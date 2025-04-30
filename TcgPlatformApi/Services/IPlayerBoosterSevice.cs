using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerBoosterService
    {
        Task<bool> AddBoosterAsync(List<PlayerBoosterRequest> requests);
        Task<List<Booster>> GetPlayerBoostersAsync(int playerId);
        Task<bool> RemoveBoosterAsync(List<PlayerBoosterRequest> requests);
    }
}
