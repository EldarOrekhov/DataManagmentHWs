using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConsoleApp1;

class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedData(db);

            var lastMonth = DateTime.Now.AddMonths(-1);
            var topCustomers = db.Orders
                .Where(o => o.OrderDate >= lastMonth)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(c => c.OrderCount) 
                .Take(5)
                .Join(db.Customers, o => o.CustomerId, c => c.CustomerId, (o, c) => new
                {
                    c.Name,
                    o.OrderCount
                })
                .ToList();

            Console.WriteLine("Top 5 customers:");
            foreach (var customer in topCustomers)
            {
                Console.WriteLine($"Customer: {customer.Name}, Orders: {customer.OrderCount}");
            }
        }
    }

    public static void SeedData(ApplicationContext db)
    {
        var customers = new[]
        {
            new Customer { Name = "Customer1" },
            new Customer { Name = "Customer2" },
            new Customer { Name = "Customer3" },
            new Customer { Name = "Customer4" },
            new Customer { Name = "Customer5" }
        };
        db.Customers.AddRange(customers);

        var orders = new[]
        {
            new Order { CustomerId = 1, OrderDate = DateTime.Now.AddDays(-5), TotalAmount = 100.00m },
            new Order { CustomerId = 1, OrderDate = DateTime.Now.AddDays(-10), TotalAmount = 50.00m },
            new Order { CustomerId = 2, OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 75.00m },
            new Order { CustomerId = 3, OrderDate = DateTime.Now.AddDays(-7), TotalAmount = 120.00m },
            new Order { CustomerId = 3, OrderDate = DateTime.Now.AddDays(-15), TotalAmount = 80.00m },
            new Order { CustomerId = 4, OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 200.00m },
            new Order { CustomerId = 5, OrderDate = DateTime.Now.AddDays(-3), TotalAmount = 150.00m }
        };
        db.Orders.AddRange(orders);

        db.SaveChanges();
    }
}

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=OrdersDb;Trusted_Connection=True;");
    }
}
