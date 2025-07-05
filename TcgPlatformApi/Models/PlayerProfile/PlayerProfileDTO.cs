namespace TcgPlatformApi.Models
{
    public class PlayerProfileDTO
    {
        public required string Nickname { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public int Experience { get; set; }
        public required string AvatarPath { get; set; }
        public int BattlesPlayed { get; set; }
    }
}
