namespace TcgPlatformApi.Models
{
    public class PlayerDeckRequest
    {
        public int DeckId { get; set; }
        public string DeckName { get; set; }
        public int PlayerId { get; set; }
        public List<PlayerDeckCardDTO> Cards { get; set; }

        public PlayerDeckRequest()
        {
            Cards = new List<PlayerDeckCardDTO>();
        }
    }
}