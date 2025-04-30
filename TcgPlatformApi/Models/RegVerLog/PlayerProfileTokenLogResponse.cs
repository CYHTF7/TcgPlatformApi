namespace TcgPlatformApi.Models
{
    public class PlayerProfileTokenLogResponse
    {
        public PlayerProfileDTO PlayerProfile { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
