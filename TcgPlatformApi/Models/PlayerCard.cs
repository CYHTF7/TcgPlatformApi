namespace TcgPlatformApi.Models
{
    public class PlayerCard
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }
    }
}