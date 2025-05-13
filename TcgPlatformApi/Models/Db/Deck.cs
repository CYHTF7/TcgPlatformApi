namespace TcgPlatformApi.Models
{
    public class Deck
    {
        public int Id { get; set; } //DeckId   
        public required string DeckName { get; set; }
        public int PlayerId { get; set; }

        public List<DeckCard> PlayerDeckCards { get; set; } = new();
    }
}
