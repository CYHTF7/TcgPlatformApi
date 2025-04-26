using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using TcgPlatformApi.Data;

namespace TcgPlatformApi.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(string toEmail, string code, int variant)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("wowtcgonline@gmail.com", "uoce qpok olkn rgzj"),
                EnableSsl = true,
            };

            if (variant == 1)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("wowtcgonline@gmail.com", "WOWTCGONLINE"),
                    Subject = "WOWTCG - Verification Code",
                    Body = $"Your Verification Code: {code}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }

            if (variant == 2)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("wowtcgonline@gmail.com", "WOWTCGONLINE"),
                    Subject = "WOWTCG - Reset Password Code",
                    Body = $"Your Reset Password Code: {code}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }

            return true;
        }
    }
}
