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

        try
        {
            string avatarUrl = await _avatarService.UploadAvatar(file, parsedUserId);
            return Ok(new { url = avatarUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetAvatar(string userId)
    {
        if (!int.TryParse(userId, out int parsedUserId))
        {
            return BadRequest("Invalid userId!");
        }

        try
        {
            byte[] avatarBytes = await _avatarService.GetAvatar(parsedUserId);
            return File(avatarBytes, "image/jpeg");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

