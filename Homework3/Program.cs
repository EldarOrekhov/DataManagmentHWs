using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1;
class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Description { get; set; }
    public string TemporaryData { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ShopDb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(e => e.Id);
            e.Property(e => e.Name).HasMaxLength(100);
            e.Property(e => e.Name).IsRequired();
            e.Property(e => e.Price).HasPrecision(10, 2);
            e.Property(e => e.StockQuantity).HasDefaultValue(0);
            e.Property(e => e.Description).IsRequired(false);
            e.HasIndex(e => e.Name).IsUnique();
            e.Ignore(e => e.TemporaryData);
            e.ToTable("StoreProducts");
            e.ToTable(e => e.HasCheckConstraint("Price", "Price >= 0"));
        });

        base.OnModelCreating(modelBuilder);
    }
}