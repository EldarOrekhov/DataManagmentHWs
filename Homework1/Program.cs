using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1;
internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(connectionString);

        //using (ApplicationContext db = new ApplicationContext(optionsBuilder.Options))
        //{
        //    db.Database.EnsureDeleted();
        //    db.Database.EnsureCreated();

        //    Group group = new Group { Name = "Group1" };
        //    Student student = new Student { Name = "Student1", Groups = new List<Group> { group } };
        //    db.Groups.Add(group);
        //    db.Students.Add(student);
        //    db.SaveChanges();
        //}

        //Additional task 2
        ApplicationContext db = new ApplicationContext(optionsBuilder.Options);
        var student = db.Students.Include(s => s.Groups).FirstOrDefault(s => s.Id == 1);
        if (student != null)
        {
            student.Name = "Student2";
            db.SaveChanges();
            Console.WriteLine($"Student 1 updated");
        }
        db.Dispose();
    }

    public static List<Group> GetStudentGroups(ApplicationContext db, int studentId)
    {
        var student = db.Students.Include(s => s.Groups).FirstOrDefault(s => s.Id == studentId);
        return student?.Groups ?? new List<Group>();
    }
    public static List<Student> GetGroupStudents(ApplicationContext db, int groupId)
    {
        var group = db.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId);
        return group?.Students ?? new List<Student>();
    }

    static void UpdateStudent(ApplicationContext db, int studentId, string newName, List<Group> newGroups = null)
    {
        var student = db.Students.FirstOrDefault(s => s.Id == studentId);
        if (student != null)
        {
            student.Name = newName;
            if (newGroups != null)
                student.Groups = newGroups;       
            db.SaveChanges();
            Console.WriteLine($"Student {studentId} updated");
        }
        else
            Console.WriteLine($"Student {studentId} not found");
    }
    static void DeleteStudent(ApplicationContext db, int studentId)
    {
        var student = db.Students.FirstOrDefault(s => s.Id == studentId);
        if (student != null)
        {
            db.Students.Remove(student);
            db.SaveChanges();
            Console.WriteLine($"Student {studentId} deleted");
        }
        else
            Console.WriteLine($"Student {studentId} not found");
    }
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Group> Groups { get; set; } = new List<Group>();

    public override string ToString()
    {
        var groupNames = Groups.Count > 0 ? "Groups: " + string.Join(", ", Groups.Select(g => g.Name)) : "No groups";
        return $"Student(Id={Id}, Name={Name}, {groupNames})";
    }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();

    public override string ToString()
    {
        var studentNames = Students.Count > 0 ? "Students: " + string.Join(", ", Students.Select(s => s.Name)) : "No students";
        return $"Group(Id={Id}, Name={Name}, {studentNames})";
    }
}

public class ApplicationContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public ApplicationContext()
    {
        Database.EnsureCreated();
    }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}