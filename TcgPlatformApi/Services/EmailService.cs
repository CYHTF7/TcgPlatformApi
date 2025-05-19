using System.Net.Mail;
using System.Net;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Smtp;

namespace TcgPlatformApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClient _smtpClient;

        public EmailService(ISmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string code, int variant)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(code))
            {
                throw new AppException(
                    userMessage: "Email was not sent",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[EmailService] Invalid email request: {variant}"
                );
            }

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

                await _smtpClient.SendMailAsync(mailMessage);

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

                await _smtpClient.SendMailAsync(mailMessage);

                return true;
            }

            return false;
        }
    }
}
