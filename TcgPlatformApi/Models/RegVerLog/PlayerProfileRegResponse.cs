namespace TcgPlatformApi.Models
{
    public class PlayerProfileRegResponse
    {
        public int Id { get; set; }
        public required string Nickname { get; set; }
        public required string Email { get; set; }
    }
}
