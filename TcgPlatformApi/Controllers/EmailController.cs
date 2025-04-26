using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Services;

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
        public async Task<IActionResult> SendEmailAsync(string toEmail, string code, int variant)
        {
            bool result = await _emailService.SendEmailAsync(toEmail, code, variant);

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
