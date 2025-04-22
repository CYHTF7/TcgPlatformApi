using System.ComponentModel.DataAnnotations.Schema;

namespace TcgPlatformApi.Models
{
    public class PlayerDeckCard
    {
        public int Id { get; set; }       
        public int DeckId { get; set; }   
        public int CardId { get; set; }   
        public int Quantity { get; set; }


        [ForeignKey("DeckId")]
        public PlayerDeck Deck { get; set; }
    }
}
