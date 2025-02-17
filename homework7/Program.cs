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
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var companies = new List<Company>
            //{
            //    new Company { Name = "Company A", Address = "Street 1, City X" },
            //    new Company { Name = "Company B", Address = "Street 2, City Y" }
            //};

            //var users = new List<User>
            //{
            //    new User { FirstName = "User1", LastName = "Last1", Age = 28, Company = companies[0] },
            //    new User { FirstName = "User2", LastName = "Last2", Age = 35, Company = companies[1] },
            //    new User { FirstName = "User3", LastName = "Last3", Age = 42, Company = companies[0] },
            //    new User { FirstName = "User4", LastName = "Last4", Age = 31, Company = companies[1] }
            //};

            //db.Companies.AddRange(companies);
            //db.Users.AddRange(users);
            //db.SaveChanges();

            //1
            var userCompanyData = db.Set<UserCompanyView>()
                .FromSqlRaw("EXEC GetUserCompanyData")
                .ToList();

            //2
            var namePattern = "User1%";
            var usersByName = db.Users
                .FromSqlRaw("EXEC GetUsersByName @p0", namePattern)
                .ToList();

            //3
            var averageAgeParameter = new SqlParameter("@AverageAge", SqlDbType.Float)
            {
                Direction = ParameterDirection.Output
            };

            db.Database.ExecuteSqlRaw("EXEC GetAverageAge @AverageAge OUT", averageAgeParameter);

            var averageAge = (double)averageAgeParameter.Value;
            Console.WriteLine($"Средний возраст пользователей: {averageAge}");

        }
    }
}

public class UserCompanyView
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
}


public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<User> Users { get; set; } = new List<User>();
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<UserCompanyView> UserCompanyViews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=usersDB;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCompanyView>().ToView("UserCompanyView");
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(e => e.Users)
            .HasForeignKey(u => u.CompanyId);
    }
}
