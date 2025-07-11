using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("getprofile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var player = await _profileService.GetProfile(id);
            return Ok(player);
        }

        [HttpPost("createprofile")]
        public async Task<IActionResult> CreateProfile([FromBody] PlayerProfile newProfile)
        {
            var createProfile = await _profileService.CreateProfile(newProfile);
            return CreatedAtAction(nameof(GetProfile), new { id = createProfile.Id }, createProfile);
        }

        [Authorize]
        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] PlayerProfileDTO updatedProfile) 
        {
            var profileId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(profileId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var updated = await _profileService.UpdateProfile(updatedProfile, parsedPlayerId);
            return Ok(updated);
        }
    }
}