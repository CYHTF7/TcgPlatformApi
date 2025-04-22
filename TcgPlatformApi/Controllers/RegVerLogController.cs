using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using System.Net.Mail;
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

        //REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegRequest request)
        {
            try
            {
                var result = await _regVerLogService.Register(request);
                return Ok(result); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        //EMAIL
        [HttpPost("email")]
        public async Task<IActionResult> SendEmailAsync(string toEmail, string code, int variant)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //VER
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerRequest request)
        {
            try
            {
                await _regVerLogService.VerifyAccount(request);
                return Ok(); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        //LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogRequest request)
        {
            try
            {
                var result = await _regVerLogService.Login(request);
                return Ok(result); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (VerificationException ex)
            {
                return Unauthorized(ex.Message); 
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        //RESSPASS
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] RessPassRequest request)
        {
            try
            {
                await _regVerLogService.ResetPassword(request);
                return Ok("Password reset email sent.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (VerificationException ex)
            {
                return Unauthorized(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //VERRESSPASS
        [HttpPost("verifyresetpassword")]
        public async Task<IActionResult> VerifyResetPassword([FromBody] VerRessPassRequest request)
        {
            try
            {
                await _regVerLogService.VerifyResetPassword(request);
                return Ok("Password successfully changed.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
