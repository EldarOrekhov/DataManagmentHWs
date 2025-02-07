using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();
            //List<Company> companies = new List<Company>
            //{
            //    new Company { Name = "Company1" },
            //    new Company { Name = "Company2" }
            //};
            //List<Employee> employees = new List<Employee>
            //{
            //    new Employee { Name = "Employee1", Company = companies[0] },
            //    new Employee { Name = "Employee2", Company = companies[0] },
            //    new Employee { Name = "Employee3", Company = companies[1] },
            //    new Employee { Name = "Employee4", Company = companies[1] }
            //};
            //List<Project> projects = new List<Project>
            //{
            //    new Project { Name = "ProjectA" },
            //    new Project { Name = "ProjectB" },
            //    new Project { Name = "ProjectC" }
            //};

            //employees[0].Projects.Add(projects[0]);
            //employees[1].Projects.Add(projects[0]);
            //employees[1].Projects.Add(projects[1]);
            //employees[2].Projects.Add(projects[1]);
            //employees[3].Projects.Add(projects[2]); 

            //db.Companies.AddRange(companies);
            //db.Employees.AddRange(employees);
            //db.Projects.AddRange(projects);
            //db.SaveChanges();

            Console.WriteLine($"Company1 projects:");
            var Company1Projects = db.Projects.Include(e => e.Employees).ThenInclude(e => e.Company)
             .Where(e => e.Employees.Any(e => e.Company.Name == "Company1"))
             .Select(e => e).ToList();
            foreach (Project project in Company1Projects)
            {
                Console.WriteLine($"{project.Name}");
            }
        }
    }
}

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public List<Project> Projects { get; set; } = new List<Project>();
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
}

public class ApplicationContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=companydb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Company)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.CompanyId);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Projects)
            .WithMany(e => e.Employees)
            .UsingEntity(e => e.ToTable("EmployeeProjects"));

        base.OnModelCreating(modelBuilder);
    }
}