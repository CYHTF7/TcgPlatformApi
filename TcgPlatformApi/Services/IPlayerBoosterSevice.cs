using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerBoosterService
    {
        Task<bool> AddBoosterAsync(List<PlayerBoosterRequest> requests);
        Task<List<PlayerBooster>> GetPlayerBoostersAsync(int playerId);
        Task<bool> RemoveBoosterAsync(List<PlayerBoosterRequest> requests);
    }
}
