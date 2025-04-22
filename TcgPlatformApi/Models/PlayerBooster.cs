namespace TcgPlatformApi.Models
{
    public class PlayerBooster
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BoosterId { get; set; }
        public int Quantity { get; set; }
    }
}