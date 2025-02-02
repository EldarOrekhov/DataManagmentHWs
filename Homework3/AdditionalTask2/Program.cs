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
    private string _name;
    public int Id { get; set; }
    public string Name { get => _name; set => _name = value; }    
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public Product(string name, decimal price, string category, string description)
    {
        Name = name;
        Price = price;
        Category = category;
        Description = description;
    }
}

public class ApplicationContext : DbContext
{
    public DbSet<Product> products { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ProductsDb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(e => new { e.Id, e.Name });
            e.Property(e => e.Name).IsRequired().HasMaxLength(100);
            e.ToTable(e => e.HasCheckConstraint("Price", "Price >= 0"));
        });

        base.OnModelCreating(modelBuilder); 
    }
}
