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

            var departmentInfo = db.Departments
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    AverageSalary = d.Employees.Average(e => e.Salary),
                    TopEmployees = d.Employees
                        .OrderByDescending(e => e.Salary)
                        .Take(3)
                        .Select(e => new
                        {
                            e.Salary
                        })
                        .ToList()
                })
                .ToList();

            foreach (var department in departmentInfo)
            {
                Console.WriteLine($"Department: {department.DepartmentName}, Avg Salary: {department.AverageSalary}");
                Console.WriteLine("Top 3 highest paid employees:");
                foreach (var employee in department.TopEmployees)
                {
                    Console.WriteLine($"\tSalary: {employee.Salary}");
                }
                Console.WriteLine();
            }
        }
    }

    public static void SeedData(ApplicationContext db)
    {
        var departments = new[]
        {
            new Department { Name = "HR" },
            new Department { Name = "IT" },
            new Department { Name = "Finance" }
        };
        db.Departments.AddRange(departments);

        var employees = new[]
        {
            new Employee { Name = "Employee1", Salary = 60000m, DepartmentId = 1 },
            new Employee { Name = "Employee2", Salary = 80000m, DepartmentId = 1 },
            new Employee { Name = "Employee3", Salary = 70000m, DepartmentId = 1 },
            new Employee { Name = "Employee4", Salary = 120000m, DepartmentId = 2 },
            new Employee { Name = "Employee5", Salary = 100000m, DepartmentId = 2 },
            new Employee { Name = "Employee6", Salary = 95000m, DepartmentId = 2 },
            new Employee { Name = "Employee7", Salary = 70000m, DepartmentId = 3 },
            new Employee { Name = "Employee8", Salary = 85000m, DepartmentId = 3 },
            new Employee { Name = "Employee9", Salary = 80000m, DepartmentId = 3 }
        };
        db.Employees.AddRange(employees);

        db.SaveChanges();
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CompanyDb;Trusted_Connection=True;");
    }
}
