﻿using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile> GetProfile(int id) 
        {
            var player = await _context.PlayerProfiles.FindAsync(id);

            if (player == null)
            {
                throw new AppException(
                    userMessage: "Player profile not found",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[ProfileService] Player profile not found: {player}"
                );
            }

            return player;
        }

        public async Task<PlayerProfile> CreateProfile(PlayerProfile newProfile) 
        {
            _context.PlayerProfiles.Add(newProfile);

            await _context.SaveChangesAsync();

            return newProfile;
        }

        public async Task<PlayerProfileDTO> UpdateProfile(PlayerProfileDTO updatedProfile, int profileId) 
        {
            var existingPlayer = await _context.PlayerProfiles.FindAsync(profileId);

            if (existingPlayer == null)
            {
                throw new AppException(
                    userMessage: "Player profile is missing",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[ProfileService] Player profile is missing: {existingPlayer}"
                );
            }

            existingPlayer.Nickname = updatedProfile.Nickname;
            existingPlayer.Level = updatedProfile.Level;
            existingPlayer.Gold = updatedProfile.Gold;
            existingPlayer.Experience = updatedProfile.Experience;
            existingPlayer.AvatarPath = updatedProfile.AvatarPath;
            existingPlayer.BattlesPlayed = updatedProfile.BattlesPlayed;

            await _context.SaveChangesAsync();

            return new PlayerProfileDTO
            {
                Nickname = existingPlayer.Nickname,
                Level = existingPlayer.Level,
                Gold = existingPlayer.Gold,
                Experience = existingPlayer.Experience,
                AvatarPath = existingPlayer.AvatarPath,
                BattlesPlayed = existingPlayer.BattlesPlayed
            };
        }
    }
}
