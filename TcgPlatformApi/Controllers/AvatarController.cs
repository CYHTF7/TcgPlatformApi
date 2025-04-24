using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Data;
using TcgPlatformApi.Services;

[ApiController]
[Route("api/avatar")]
public class AvatarController : ControllerBase
{
    private readonly IAvatarService _avatarService;

    public AvatarController(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file, [FromForm] string playerId)
    {
        if (!int.TryParse(playerId, out int parsedPlayerId))
        {
            return BadRequest("Invalid userId!");
        }

        string avatarUrl = await _avatarService.UploadAvatar(file, parsedPlayerId);
        return Ok(new { url = avatarUrl });   
    }

    [HttpGet("get/{playerId}")]
    public async Task<IActionResult> GetAvatar(string playerId)
    {
        if (!int.TryParse(playerId, out int parsedPlayerId))
        {
            return BadRequest("Invalid userId!");
        }

        byte[] avatarBytes = await _avatarService.GetAvatar(parsedPlayerId);
        return File(avatarBytes, "image/jpeg");
    }
}

