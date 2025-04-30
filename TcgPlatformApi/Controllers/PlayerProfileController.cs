using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerProfileController : ControllerBase
    {
        private readonly IPlayerProfileService _profileService;

        public PlayerProfileController(IPlayerProfileService profileService)
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

        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] PlayerProfileDTO updatedProfile) 
        {
            var updated = await _profileService.UpdateProfile(updatedProfile);
            return Ok(updated);
        }
    }
}