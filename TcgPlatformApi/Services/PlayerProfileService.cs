using TcgPlatformApi.Data;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly AppDbContext _context;

        public PlayerProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile> GetProfile(int id) 
        {
            var player = await _context.PlayerProfiles.FindAsync(id);

            if (player == null)
            {
                throw new KeyNotFoundException("Player profile not found!");
            }

            return player;
        }

        public async Task<PlayerProfile> CreateProfile(PlayerProfile newProfile) 
        {
            _context.PlayerProfiles.Add(newProfile);

            await _context.SaveChangesAsync();

            return newProfile;
        }

        public async Task<PlayerProfile> UpdateProfile(PlayerProfileRequest updatedProfile) 
        {
            var existingPlayer = await _context.PlayerProfiles.FindAsync(updatedProfile.Id);

            if (existingPlayer == null)
            {
                throw new KeyNotFoundException("Player profile not found");
            }

            existingPlayer.Nickname = updatedProfile.Nickname;
            existingPlayer.Level = updatedProfile.Level;
            existingPlayer.Gold = updatedProfile.Gold;
            existingPlayer.Experience = updatedProfile.Experience;
            existingPlayer.AvatarPath = updatedProfile.AvatarPath;
            existingPlayer.BattlesPlayed = updatedProfile.BattlesPlayed;

            await _context.SaveChangesAsync();

            return existingPlayer;
        }
    }
}
