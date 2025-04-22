namespace TcgPlatformApi.Models
{
    public class VerRessPassRequest
    {  
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }

        public string NewPassword { get; set; }
    }
}
