namespace TcgPlatformApi.Models
{
    public class PlayerCardRequest
    {
        public int PlayerId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }
    }
}