namespace TcgPlatformApi.Models
{
    public class DeckRequest
    {
        public int DeckId { get; set; }
        public required string DeckName { get; set; }
        public List<CardInDeck> Cards { get; set; }

        public DeckRequest()
        {
            Cards = new List<CardInDeck>();
        }
    }
}
