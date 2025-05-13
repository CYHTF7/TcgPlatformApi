namespace TcgPlatformApi.Models
{
    public class RefreshTokenRequest
    {
        public int PlayerId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
