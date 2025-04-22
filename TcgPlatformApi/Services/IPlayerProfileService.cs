using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IPlayerProfileService
    {
        Task<PlayerProfile> GetProfile(int id);

        Task<PlayerProfile> CreateProfile(PlayerProfile newProfile);

        Task<PlayerProfile> UpdateProfile(PlayerProfileRequest updatedProfile);
    }
}
