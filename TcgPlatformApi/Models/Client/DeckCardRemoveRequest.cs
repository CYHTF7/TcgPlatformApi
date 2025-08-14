namespace TcgPlatformApi.Models
{
    public class DeckCardRemoveRequest
    {
        public int DeckId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }

    }
}
