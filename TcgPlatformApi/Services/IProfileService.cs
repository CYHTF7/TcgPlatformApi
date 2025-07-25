﻿using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IProfileService
    {
        Task<PlayerProfile> GetProfile(int id);

        Task<PlayerProfile> CreateProfile(PlayerProfile newProfile);

        Task<PlayerProfileDTO> UpdateProfile(PlayerProfileDTO updatedProfile, int profileId);
    }
}
