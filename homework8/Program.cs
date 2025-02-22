using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var db = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=TaskDb;Trusted_Connection=True;");

            var task = new TaskModel { Title = "TestTask", Description = "Description", DueDate = DateTime.UtcNow, IsCompleted = false };
            var id = await AddTaskAsync(db, task);
            Console.WriteLine($"Task added with ID: {id}");

            var retrievedTask = await GetTaskByIdAsync(db, id);
            Console.WriteLine($"Retrieved Task: {retrievedTask.Title}");

            retrievedTask.IsCompleted = true;
            await UpdateTaskAsync(db, retrievedTask);
            Console.WriteLine("Task updated successfully");

            await DeleteTaskAsync(db, id);
            Console.WriteLine("Task deleted successfully");
        }

        public static async Task<int> AddTaskAsync(IDbConnection db, TaskModel task)
        {
            const string query = "INSERT INTO Tasks (Title, Description, DueDate, IsCompleted) VALUES (@Title, @Description, @DueDate, @IsCompleted); SELECT CAST(SCOPE_IDENTITY() as int);";
            return await db.ExecuteScalarAsync<int>(query, task);
        }

        public static async Task<TaskModel> GetTaskByIdAsync(IDbConnection db, int id)
        {
            const string query = "SELECT * FROM Tasks WHERE Id = @Id";
            return await db.QueryFirstOrDefaultAsync<TaskModel>(query, new { Id = id });
        }

        public static async Task<bool> UpdateTaskAsync(IDbConnection db, TaskModel task)
        {
            const string query = "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate, IsCompleted = @IsCompleted WHERE Id = @Id";
            return await db.ExecuteAsync(query, task) > 0;
        }

        public static async Task<bool> DeleteTaskAsync(IDbConnection db, int id)
        {
            const string query = "DELETE FROM Tasks WHERE Id = @Id";
            return await db.ExecuteAsync(query, new { Id = id }) > 0;
        }
    }
}

public class TaskModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}
