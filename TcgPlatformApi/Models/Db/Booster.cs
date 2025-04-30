namespace TcgPlatformApi.Models
{
    public class Booster
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BoosterId { get; set; }
        public int Quantity { get; set; }
    }
}