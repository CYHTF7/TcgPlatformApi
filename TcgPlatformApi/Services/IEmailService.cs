namespace TcgPlatformApi.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string code, int variant);
    }
}
