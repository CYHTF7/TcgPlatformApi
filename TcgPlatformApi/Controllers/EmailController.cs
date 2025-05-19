using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Filters;
using TcgPlatformApi.Services;
using TcgPlatformApi.Models;


namespace TcgPlatformApi.Controllers
{
    [ApiController]
    [Route("api/email")]

    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("email")]
        [ValidateEmailDomain]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequest request)
        {
            bool result = await _emailService.SendEmailAsync(request.ToEmail, request.Code, request.Variant);

            if (result)
            {
                return Ok("Email sent successfully.");
            }
            else
            {
                return BadRequest("Failed to send email.");
            }
        }
    }
}
