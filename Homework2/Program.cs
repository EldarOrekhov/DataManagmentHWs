using Microsoft.EntityFrameworkCore;
using NLog;
using OrderServiceClass;

namespace ConsoleApp1;
class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            OrderService service = new OrderService(db);
            List<Product> products = new List<Product> 
            {
                new Product {Name="Milk", Price=40},
                new Product {Name="Cheese", Price=70},
                new Product {Name="Bread", Price=20}
            };
            service.AddOrder(products, "+380930000000");
            service.AddOrder(new List<Product> { products[0], products[2] }, "+380730000000");
            service.AddOrder(new List<Product> { products[1]}, "+380630000000");
            service.ShowOrders();
            service.DeleteOrder(1);
            service.ShowOrders();
        }
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; } = DateTime.Now;
    public string Phone { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}

public class ApplicationContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ShopDb;Trusted_Connection=True;");
    }
}