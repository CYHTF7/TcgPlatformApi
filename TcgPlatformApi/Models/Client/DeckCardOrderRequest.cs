namespace TcgPlatformApi.Models
{
    public class DeckCardOrderRequest
    {
        public int DeckId { get; set; }
        public int CardId { get; set; }

        public int Order { get; set; }
    }
}
