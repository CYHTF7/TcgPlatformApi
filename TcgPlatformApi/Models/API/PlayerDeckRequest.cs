namespace TcgPlatformApi.Models
{
    public class PlayerDeckRequest
    {
        public int DeckId { get; set; }
        public required string DeckName { get; set; }
        public int PlayerId { get; set; }
        public List<CardInDeck> Cards { get; set; }

        public PlayerDeckRequest()
        {
            Cards = new List<CardInDeck>();
        }
    }
}