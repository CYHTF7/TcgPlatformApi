using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        public DbSet<PlayerCard> PlayerCards { get; set; }
        public DbSet<PlayerBooster> PlayerBoosters { get; set; }
        public DbSet<PlayerDeck> PlayerDecks { get; set; }
        public DbSet<PlayerDeckCard> PlayerDeckCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerDeck>()
                .HasMany(d => d.PlayerDeckCards)
                .WithOne(c => c.Deck)
                .HasForeignKey(c => c.DeckId);
        }
    }
}

//dotnet ef migrations add AddRegistrationFields
//dotnet ef database update