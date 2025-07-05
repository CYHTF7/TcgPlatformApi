namespace TcgPlatformApi.Models
{
    public class AuthPlayerProfileDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
