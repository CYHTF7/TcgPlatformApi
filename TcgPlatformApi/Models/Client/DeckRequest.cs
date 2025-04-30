namespace TcgPlatformApi.Models
{
    public class DeckRequest
    {
        public int DeckId { get; set; }
        public string DeckName { get; set; }
        public List<PlayerDeckCardDTO> Cards { get; set; }

        public DeckRequest()
        {
            Cards = new List<PlayerDeckCardDTO>();
        }
    }
}
