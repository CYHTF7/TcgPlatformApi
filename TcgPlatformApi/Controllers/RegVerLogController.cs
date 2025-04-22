using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using TcgPlatformApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TcgPlatformApi.Services;
using System.Security.Authentication;
using System.Security;

namespace TcgPlatformApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("email")]
        public async Task<IActionResult> SendEmailAsync(string toEmail, string code, int variant)
        {
            bool result = await _regVerLogService.SendEmailAsync(toEmail, code, variant);

            if (result)
            {
                return Ok("Email sent successfully.");
            }
            else
            {
                return BadRequest("Failed to send email.");
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerRequest request)
        {
            await _regVerLogService.VerifyAccount(request);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogRequest request)
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
