using System.ComponentModel.DataAnnotations.Schema;

namespace TcgPlatformApi.Models
{
    public class DeckCard
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }


        [ForeignKey("DeckId")]
        public Deck? Deck { get; set; }
    }
}
