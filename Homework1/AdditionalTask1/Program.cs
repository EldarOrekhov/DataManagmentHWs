using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1;
internal class Program
{
    static void Main(string[] args)
    {
        using (GroupsStudentsdbContext db = new GroupsStudentsdbContext())
        {
            List<Student> students = db.Students.ToList();
            foreach (Student student in students) 
            {
                Console.WriteLine(student.Name);
            }
            Student newStudent = new Student { Name="Student3", Groups = new List<Group> { new Group {Name="Group2" } } };
            db.Students.Add(newStudent);
            Group GroupUpdate = db.Groups.FirstOrDefault(g => g.Id == 1);
            if (GroupUpdate != null)
                GroupUpdate.Name = "Group3";
            db.SaveChanges();
        }
    }
}