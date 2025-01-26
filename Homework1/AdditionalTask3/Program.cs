using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GameStudioClasses;
using GameStudioDbContext;

namespace ConsoleApp1;
internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<GameStudioContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using (GameStudioContext db = new GameStudioContext(optionsBuilder.Options))
        {
            db.Database.EnsureCreated();
            db.Games.AddRange(
                new Game { Name = "Dota 2", Developer = "Valve", Genre = "MOBA", ReleaseDate = new DateTime(2013, 7, 9) },
                new Game { Name = "Counter-Strike 2", Developer = "Valve", Genre = "Shooter", ReleaseDate = new DateTime(2023, 9, 27) },
                new Game { Name = "World of Warcraft", Developer = "Blizzard Entertainment", Genre = "MMORPG", ReleaseDate = new DateTime(2004, 11, 23) }
            );
            db.SaveChanges();

            Console.WriteLine("\nСписок всех игр:");
            foreach (Game game in db.Games)
            {
                Console.WriteLine(game);
            }

            Console.WriteLine("\nИгры от студии Valve:");
            List<Game> gamesValve = db.Games.Where(g => g.Developer == "Valve").ToList();
            foreach (Game game in gamesValve)
            {
                Console.WriteLine(game);
            }
        }
    }
}