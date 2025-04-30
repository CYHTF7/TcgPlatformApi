namespace TcgPlatformApi.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string RefreshTokenSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresInHours { get; set; }
        public int RefreshTokenExpiresInDays { get; set; }
    }
}
