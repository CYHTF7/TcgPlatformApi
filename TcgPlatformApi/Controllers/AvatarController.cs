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
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file, [FromForm] string userId)
    {
        if (!int.TryParse(userId, out int parsedUserId))
        {
            return BadRequest("Invalid userId!");
        }

        string avatarUrl = await _avatarService.UploadAvatar(file, parsedUserId);
        return Ok(new { url = avatarUrl });   
    }

    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetAvatar(string userId)
    {
        if (!int.TryParse(userId, out int parsedUserId))
        {
            return BadRequest("Invalid userId!");
        }

        byte[] avatarBytes = await _avatarService.GetAvatar(parsedUserId);
        return File(avatarBytes, "image/jpeg");
    }
}

