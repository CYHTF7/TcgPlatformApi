namespace TcgPlatformApi.Models
{
    public class VerRessPassRequest
    {  
        public required string Email { get; set; }
        public required string PasswordResetCode { get; set; }
        public required string NewPassword { get; set; }
    }
}
