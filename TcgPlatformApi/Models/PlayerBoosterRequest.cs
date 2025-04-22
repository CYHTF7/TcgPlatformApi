namespace TcgPlatformApi.Models
{
    public class PlayerBoosterRequest
    {
        public int PlayerId { get; set; }
        public int BoosterId { get; set; }
        public int Quantity { get; set; }
    }
}
