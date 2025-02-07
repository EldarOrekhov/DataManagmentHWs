using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Country country = new Country { Name = "Country1" };
            db.Countries.Add(country);
            db.SaveChanges();

            Airport airport = new Airport { Name = "Airport1", CountryId = country.Id };
            db.Airports.Add(airport);
            db.SaveChanges();

            Airplane airplane = new Airplane { Model = "Airplane1", AirportId = airport.Id };
            AirplaneFeatures features = new AirplaneFeatures { MaxSpeed = 900, Capacity = 400, Airplane = airplane };

            db.Airplanes.Add(airplane);
            db.Features.Add(features);
            db.SaveChanges();

            Console.WriteLine("Airplane info:");
            var airplaneData = db.Airplanes
                .Include(e => e.Features)
                .Include(e => e.Airport)
                .ThenInclude(e => e.Country)
                .FirstOrDefault(e => e.Id == airplane.Id);

            if (airplaneData != null)
            {
                Console.WriteLine($"Model: {airplaneData.Model}");
                Console.WriteLine($"Features:\n\tSpeed: {airplaneData.Features.MaxSpeed} km/h, Capacity {airplaneData.Features.Capacity}");
                Console.WriteLine($"Airport: {airplaneData.Airport.Name}");
                Console.WriteLine($"Country: {airplaneData.Airport.Country.Name}");
            }

            Console.WriteLine("\nCountry airports:");
            var countryData = db.Countries
                .Include(e => e.Airports)
                .ThenInclude(e => e.Airplanes)
                .ThenInclude(e => e.Features)
                .FirstOrDefault(e => e.Id == country.Id);

            if (countryData != null)
            {
                Console.WriteLine($"Country: {countryData.Name}");
                foreach (Airport airports in countryData.Airports)
                {
                    Console.WriteLine($"\tAirport: {airports.Name}");
                    foreach (Airplane airplanes in airports.Airplanes)
                    {
                        Console.WriteLine($"\t\tAirplane: {airplanes.Model}");
                        Console.WriteLine($"\t\tSpeed: {airplanes.Features.MaxSpeed} km/h");
                        Console.WriteLine($"\t\tCapacity: {airplanes.Features.Capacity}");
                    }
                }
            }
        }
    }
}

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Airport> Airports { get; set; } = new List<Airport>();
}

public class Airport
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public List<Airplane> Airplanes { get; set; } = new List<Airplane>();
}

public class Airplane
{
    public int Id { get; set; }
    public string Model { get; set; }
    public int AirportId { get; set; }
    public Airport Airport { get; set; }
    public AirplaneFeatures Features { get; set; }
}

public class AirplaneFeatures
{
    public int Id { get; set; }
    public int MaxSpeed { get; set; }
    public int Capacity { get; set; }
    public int AirplaneId { get; set; }
    public Airplane Airplane { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Airplane> Airplanes { get; set; }
    public DbSet<AirplaneFeatures> Features { get; set; } 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=airportsdb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airplane>()
            .HasOne(e => e.Features)
            .WithOne(e => e.Airplane)
            .HasForeignKey<AirplaneFeatures>(e => e.AirplaneId);

        base.OnModelCreating(modelBuilder);
    }
}