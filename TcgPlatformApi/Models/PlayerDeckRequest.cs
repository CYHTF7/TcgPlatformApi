namespace TcgPlatformApi.Models
{
    public class PlayerDeckRequest
    {
        public int DeckId { get; set; }
        public string DeckName { get; set; }
        public int PlayerId { get; set; }
        public List<PlayerDeckCardRequest> Cards { get; set; }

        public PlayerDeckRequest()
        {
            Cards = new List<PlayerDeckCardRequest>();
        }
    }
}