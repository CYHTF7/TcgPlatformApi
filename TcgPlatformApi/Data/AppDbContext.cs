using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        public DbSet<Card> PlayerCards { get; set; }
        public DbSet<Booster> PlayerBoosters { get; set; }
        public DbSet<Deck> PlayerDecks { get; set; }
        public DbSet<DeckCard> PlayerDeckCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deck>()
                .HasMany(d => d.PlayerDeckCards)
                .WithOne(c => c.Deck)
                .HasForeignKey(c => c.DeckId);
        }
    }
}
//dotnet tool install --global dotnet-ef //first
//dotnet ef migrations add AddRegistrationFields
//dotnet ef database update