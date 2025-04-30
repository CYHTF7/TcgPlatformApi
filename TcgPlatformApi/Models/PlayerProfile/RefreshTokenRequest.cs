namespace TcgPlatformApi.Models
{
    public class RefreshTokenRequest
    {
        public int PlayerId { get; set; }
        public string RefreshToken { get; set; }
    }
}
