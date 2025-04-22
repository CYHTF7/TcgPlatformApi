namespace TcgPlatformApi.Models
{
    public class PlayerProfileRequest
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public int Experience { get; set; }
        public string AvatarPath { get; set; }
        public int BattlesPlayed { get; set; }
    }
}
