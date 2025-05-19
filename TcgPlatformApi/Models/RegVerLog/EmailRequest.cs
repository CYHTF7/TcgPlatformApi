namespace TcgPlatformApi.Models
{
    public class EmailRequest
    {
        public required string ToEmail { get; set; }
        public required string Code { get; set; }
        public required int Variant { get; set; }
    }
}
