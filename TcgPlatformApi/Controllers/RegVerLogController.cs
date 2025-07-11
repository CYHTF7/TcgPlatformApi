using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Route("api/regverlog")]
    [ApiController]
    public class RegVerLogController : ControllerBase
    {
        private readonly IRegVerLogService _regVerLogService;

        public RegVerLogController(IRegVerLogService regVerLogService)
        {
            _regVerLogService = regVerLogService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegRequest request)
        {
            var result = await _regVerLogService.Register(request);
            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerRequest request)
        {
            await _regVerLogService.VerifyAccount(request);
            return Ok("Verified.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] PlayerProfileLogRequest request)
        {
            var result = await _regVerLogService.Login(request);
            return Ok(result);
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] RessPassRequest request)
        {
            await _regVerLogService.ResetPassword(request);
            return Ok("Password reset email sent.");
        }

        [HttpPost("verifyresetpassword")]
        public async Task<IActionResult> VerifyResetPassword([FromBody] VerRessPassRequest request)
        {
            await _regVerLogService.VerifyResetPassword(request);
            return Ok("Password successfully changed.");
        }
    }
}
