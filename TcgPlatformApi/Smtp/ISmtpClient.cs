using System.Net.Mail;

namespace TcgPlatformApi.Smtp
{
    public interface ISmtpClient : IDisposable
    {
        Task SendMailAsync(MailMessage message);
    }
}
