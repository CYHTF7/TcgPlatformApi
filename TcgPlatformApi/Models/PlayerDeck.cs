using System.ComponentModel.DataAnnotations.Schema;

namespace TcgPlatformApi.Models
{
    public class PlayerDeck
    {
        public int Id { get; set; } //DeckId   
        public string DeckName { get; set; }     
        public int PlayerId { get; set; }

        public List<PlayerDeckCard> PlayerDeckCards { get; set; } = new();
    }
}
