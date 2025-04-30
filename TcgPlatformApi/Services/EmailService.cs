using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using TcgPlatformApi.Settings;

namespace TcgPlatformApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string code, int variant)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl,
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
