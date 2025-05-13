namespace TcgPlatformApi.Models
{
    public class PlayerProfileTokenLogResponse
    {
        public required PlayerProfileDTO PlayerProfile { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
