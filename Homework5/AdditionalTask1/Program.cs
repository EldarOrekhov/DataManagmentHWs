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
            List<Course> courses = new List<Course>
            {
                new Course { Title = "Course1", Credits = 5 },
                new Course { Title = "Course2", Credits = 4 },
                new Course { Title = "Course3", Credits = 6 },
                new Course { Title = "Course4", Credits = 3 },
                new Course { Title = "Course5", Credits = 4 }
            };
            List<Student> students = new List<Student>
            {
                new Student { Name = "Student1", DateOfBirth = new DateTime(1995, 1, 10) },
                new Student { Name = "Student2", DateOfBirth = new DateTime(1988, 5, 15) },
                new Student { Name = "Student3", DateOfBirth = new DateTime(2000, 9, 20) },
                new Student { Name = "Student4", DateOfBirth = new DateTime(1983, 11, 5) },
                new Student { Name = "Student5", DateOfBirth = new DateTime(1992, 2, 25) }
            };
            db.AddRange(courses);
            db.AddRange(students);
            db.SaveChanges();
            List<Enrollment> enrollments = new List<Enrollment>
            {
                new Enrollment { StudentId = 1, CourseId = 1, Grade = 5 },
                new Enrollment { StudentId = 1, CourseId = 2, Grade = 4 },
                new Enrollment { StudentId = 1, CourseId = 3, Grade = 3 },

                new Enrollment { StudentId = 2, CourseId = 1, Grade = 4 },
                new Enrollment { StudentId = 2, CourseId = 4, Grade = 5 },

                new Enrollment { StudentId = 3, CourseId = 2, Grade = 3 },
                new Enrollment { StudentId = 3, CourseId = 5, Grade = 4 },

                new Enrollment { StudentId = 4, CourseId = 3, Grade = 5 },

                new Enrollment { StudentId = 5, CourseId = 4, Grade = 4 }
            };
            db.Enrollments.AddRange(enrollments);
            db.SaveChanges();

            //1
            var studentCourseCount = db.Students
                .Select(e => new
                {
                    StudentName = e.Name,
                    CourseCount = e.Enrollments.Count
                }).ToList();

            //2
            var moreThan10 = db.Courses
                .Where(e => e.Enrollments.Count > 10)
                .Select(e => new
                {
                    CourseTitle = e.Title,
                    StudentCount = e.Enrollments.Count
                }).ToList();

            //2
            var studentAvg = db.Students
                .Select(e => new
                {
                    StudentName = e.Name,
                    AverageGrade = e.Enrollments.Average(e => e.Grade)
                }).ToList();

            //4
            var withoutCourses = db.Students
                .Where(s => !s.Enrollments.Any())
                .Select(s => s.Name)
                .ToList();

            //5
            var bestStudentd = db.Enrollments
                .Where(e => e.CourseId == 1)
                .OrderByDescending(e => e.Grade)
                .Select(e => new
                {
                    StudentName = e.Student.Name,
                    Grade = e.Grade
                }).FirstOrDefault();

            //6
            var students30Years = db.Students
                .Where(s => DateTime.Now.Year - s.DateOfBirth.Year > 30)
                .SelectMany(s => s.Enrollments)
                .GroupBy(e => e.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    StudentCount = g.Count()
                }).ToList();

            //7
            var maxAvg = db.Courses
                .Select(c => new
                {
                    CourseTitle = c.Title,
                    AverageGrade = c.Enrollments.Average(e => e.Grade)
                })
                .OrderByDescending(c => c.AverageGrade).FirstOrDefault();

            var minAvg = db.Courses
                .Select(c => new
                {
                    CourseTitle = c.Title,
                    AverageGrade = c.Enrollments.Average(e => e.Grade)
                })
                .OrderBy(c => c.AverageGrade).FirstOrDefault();
        }
    }
}
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Credits { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
public class Enrollment
{
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public int Grade { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=StudendtsDB;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);

        base.OnModelCreating(modelBuilder);
    }
}