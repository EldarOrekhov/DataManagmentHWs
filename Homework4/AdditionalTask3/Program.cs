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
            var file = new Menu { Name = "File" };
            var saveAs = new Menu { Name = "Save As", Parent = file };
            db.Menus.AddRange
            (
                file, new Menu { Name = "Edit" }, new Menu { Name = "View" },
                new Menu { Name = "Open", Parent = file }, new Menu { Name = "Save", Parent = file },
                saveAs, new Menu { Name = "To hard-drive..", Parent = saveAs }, new Menu { Name = "To online-drive..", Parent = saveAs 
            });
            db.SaveChanges();

            var menus = db.Menus
                .Where(e => e.ParentId == null)
                .Include(e => e.SubMenu)
                .ThenInclude(e => e.SubMenu)
                .ToList();

            Console.WriteLine("Menu:");
            PrintMenu(menus);
        }
    }

    static void PrintMenu(List<Menu> menus, int level = 0)
    {
        foreach (var menu in menus)
        {
            Console.WriteLine($"{new string('-', level + 1)}{menu.Name}");
            if (menu.SubMenu.Any())
                PrintMenu(menu.SubMenu, level + 1);
        }
    }
}

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public Menu Parent { get; set; }
    public List<Menu> SubMenu { get; set; } = new List<Menu>();
}


public class ApplicationContext : DbContext
{
    public DbSet<Menu> Menus { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=menudb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>().HasOne(m => m.Parent).WithMany(m => m.SubMenu)
             .HasForeignKey(m => m.ParentId)
             .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}