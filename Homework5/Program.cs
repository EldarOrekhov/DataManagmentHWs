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

            Instructor instructor = new Instructor { FirstName = "InstructorName", LastName = "InstructorLastName" };
            Course course1 = new Course { Title = "CourseTitle1", Description = "CourseDescription1", Instructor = instructor };
            Course course2 = new Course { Title = "CourseTitle2", Description = "CourseDescription2", Instructor = instructor };

            Student student1 = new Student { FirstName = "Name1", LastName = "LastName1", BirthDate = new DateTime(2001, 1, 01) };
            Student student2 = new Student { FirstName = "Name2", LastName = "LastName2", BirthDate = new DateTime(2002, 2, 02) };

            db.Students.AddRange(student1, student2);
            db.Courses.AddRange(course1, course2);
            db.Enrollments.AddRange(
                new Enrollment { Student = student1, Course = course1, EnrollmentDate = DateTime.Now },
                new Enrollment { Student = student2, Course = course1, EnrollmentDate = DateTime.Now },
                new Enrollment { Student = student1, Course = course2, EnrollmentDate = DateTime.Now }
            );
            db.SaveChanges();

            //1
            int courseId = course1.Id;
            var studentsInCourse = db.Enrollments.Where(e => e.CourseId == courseId).Select(e => e.Student).ToList();

            //2
            var instructorId = instructor.Id;
            var coursesByInstructor = db.Courses.Where(e => e.InstructorId == instructorId).ToList();

            //3
            var coursesWithStudents = db.Courses.Where(e => e.InstructorId == instructorId)
                .Select(e => new { Course = e, Students = e.Enrollments.Select(en => en.Student) }).ToList();

            //4
            var coursesMore5 = db.Courses.Where(e => e.Enrollments.Count() > 5).ToList();

            //5
            var older25 = db.Students.Where(e => EF.Functions.DateDiffYear(e.BirthDate, DateTime.Now) > 25).ToList();

            //6
            var avgAge = db.Students.Average(e => EF.Functions.DateDiffYear(e.BirthDate, DateTime.Now));

            //7
            var youngestStudent = db.Students.OrderBy(e => e.BirthDate).FirstOrDefault();

            var studentId = student1.Id;
            var studentCoursesCount = db.Enrollments.Count(e => e.StudentId == studentId);

            //9
            var studentNames = db.Students.Select(e => e.FirstName).ToList();

            //10
            var studentsByAge = db.Students.GroupBy(e => EF.Functions.DateDiffYear(e.BirthDate, DateTime.Now))
                .Select(e => new { Age = e.Key, Students = e.ToList() }).ToList();

            //11
            var sortedStudents = db.Students.OrderBy(e => e.LastName).ToList();

            //12
            var studentsWithEnrollments = db.Students.Select(e => new { Student = e, Enrollments = e.Enrollments }).ToList();

            //13
            var enrolledStudents = db.Enrollments.Where(e => e.CourseId == courseId).Select(e => e.StudentId);
            var studentsNotInCourse = db.Students.Where(e => !enrolledStudents.Contains(e.Id)).ToList();

            //14
            var studentsInTwoCourses = db.Students.Where(e => e.Enrollments.Count(en => en.CourseId == course1.Id || en.CourseId == course2.Id) == 2).ToList();

            //15
            var courseStudentCounts = db.Courses.Select(e => new { Course = e.Title, Count = e.Enrollments.Count() }).ToList();
        }
    }
}
public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
}

public class Instructor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>();
}

public class ApplicationContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=StudendtsDB;Trusted_Connection=True;");
    }
}