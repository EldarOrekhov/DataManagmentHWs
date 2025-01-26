using GameStudioClasses;
using Microsoft.EntityFrameworkCore;

namespace GameStudioDbContext
{
    public class GameStudioContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public GameStudioContext(DbContextOptions<GameStudioContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
