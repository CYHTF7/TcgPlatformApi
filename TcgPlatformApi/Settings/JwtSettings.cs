namespace TcgPlatformApi.Settings
{
    public class JwtSettings
    {
        public required string SecretKey { get; set; }
        public required string RefreshTokenSecret { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiresInHours { get; set; }
        public int RefreshTokenExpiresInDays { get; set; }
    }
}
