using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TcgPlatformApi.Services;
using TcgPlatformApi.Swagger;


namespace TcgPlatformApi.Controllers 
{
    [Authorize]
    [ApiController]
    [Route("api/avatar")]
    public class AvatarController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        public AvatarController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        [HttpPost("uploadavatar")]
        [SwaggerUploadFile]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            string avatarUrl = await _avatarService.UploadAvatar(file, parsedPlayerId);
            return Ok(new { url = avatarUrl });
        }

        [HttpGet("getavatar")]
        public async Task<IActionResult> GetAvatar()
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            byte[] avatarBytes = await _avatarService.GetAvatar(parsedPlayerId);
            return File(avatarBytes, "image/jpeg");
        }
    }
}


