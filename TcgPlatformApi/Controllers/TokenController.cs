using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;
namespace TcgPlatformApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/email")]

    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _tokenService.RefreshToken(request);
            return Ok(result);
        }
    }
}
