namespace TcgPlatformApi.Models
{
    public class RegRequest
    {
        public required string Nickname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
