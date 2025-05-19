using System.Net.Mail;
using System.Net;
using TcgPlatformApi.Settings;
using TcgPlatformApi.Smtp;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(SmtpSettings settings)
    {
        _smtpClient = new SmtpClient(settings.Host)
        {
            Port = settings.Port,
            Credentials = new NetworkCredential(settings.Username, settings.Password),
            EnableSsl = settings.EnableSsl,
        };
    }

    public Task SendMailAsync(MailMessage message) => _smtpClient.SendMailAsync(message);

    public void Dispose() => _smtpClient.Dispose();
}
