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

            List<User> users = new List<User>
            {
                new User {Name = "User1", UserSettings = new UserSettings { Email = "Email1@gmail.com", Country = "Country1"} },
                new User {Name = "User2", UserSettings = new UserSettings { Email = "Email2@gmail.com", Country = "Country2"} },
                new User {Name = "User3", UserSettings = new UserSettings { Email = "Email3@gmail.com", Country = "Country3"} }
            };
            db.Users.AddRange(users);
            db.SaveChanges();

            Console.WriteLine("All Users:");
            List<User> usersGet = db.Users.Include(e => e.UserSettings).ToList();
            foreach (User user in usersGet) 
            {
                Console.WriteLine($"User: { user.Name}\nUserSettings:\n\tEmail: {user.UserSettings.Email}, Country: {user.UserSettings.Country}\n");
            }

            Console.WriteLine("User2:");
            User user2 = db.Users.Include(e => e.UserSettings).FirstOrDefault(e => e.Id == 2);
            if (user2 != null)
                Console.WriteLine($"User: {user2.Name}\nUserSettings:\n\tEmail: {user2.UserSettings.Email}, Country: {user2.UserSettings.Country}\n");
            else
                Console.WriteLine("User 2 not found");

            User user3 = db.Users.Include(e => e.UserSettings).FirstOrDefault(e => e.Id == 3);
            if (user3 != null) 
            {
                db.Users.Remove(user3);
                db.SaveChanges();
            }        

            Console.WriteLine("All Users after deleting User3:");
            List<User> usersGet2 = db.Users.Include(e => e.UserSettings).ToList();
            foreach (User user in usersGet2)
            {
                Console.WriteLine($"User: {user.Name}\nUserSettings:\n\tEmail: {user.UserSettings.Email}, Country: {user.UserSettings.Country}\n");
            }
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public UserSettings UserSettings { get; set; }
}

public class UserSettings
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}


public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=userdb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasOne(e => e.UserSettings).WithOne(e => e.User).OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}