namespace TcgPlatformApi.Models
{
    public class PlayerProfileTokenLogResponse
    {
        public required PlayerProfileDTO PlayerProfile { get; set; }
        public required AuthPlayerProfileDTO AuthPlayerProfile { get; set; }
    }
}
