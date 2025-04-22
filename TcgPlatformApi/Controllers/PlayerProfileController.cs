using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Data;
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

        //GET
        [HttpGet("getprofile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                var player = await _profileService.GetProfile(id);
                return Ok(player);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //CREATE
        [HttpPost("createprofile")]
        public async Task<IActionResult> CreateProfile([FromBody] PlayerProfile newProfile)
        {
            var createProfile = await _profileService.CreateProfile(newProfile);
            return CreatedAtAction(nameof(GetProfile), new { id = createProfile.Id }, createProfile);
        }

        //UPDATE
        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] PlayerProfileRequest updatedProfile) 
        {
            try
            {
                var updated = await _profileService.UpdateProfile(updatedProfile);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Fail receive profile!");
            }
        }
    }
}