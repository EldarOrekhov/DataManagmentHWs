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
            if (!db.Authors.Any())
            {
                var authors = new List<Author>
                {
                    new Author { Name = "Author 1" },
                    new Author { Name = "Author 2" },
                    new Author { Name = "Author 3" }
                };

                db.Authors.AddRange(authors);
                db.SaveChanges();
            }

            if (!db.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book { Title = "Book 1", Price = 10.99m, AuthorId = 1 },
                    new Book { Title = "Book 2", Price = 12.99m, AuthorId = 1 },
                    new Book { Title = "Book 3", Price = 15.99m, AuthorId = 2 },
                    new Book { Title = "Book 4", Price = 9.99m, AuthorId = 2 },
                    new Book { Title = "Book 5", Price = 20.99m, AuthorId = 3 },
                    new Book { Title = "Book 6", Price = 25.99m, AuthorId = 3 }
                };

                db.Books.AddRange(books);
                db.SaveChanges();
            }
            
            db.Database.ExecuteSqlRaw("EXEC UpdateBookPricesByAuthor @AuthorId, @NewPrice",
                new SqlParameter("@AuthorId", 1),
                new SqlParameter("@NewPrice", 19.99m));

        }
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; } = new List<Book>();
}


public class ApplicationContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=BookStoreDb;Trusted_Connection=True;");
    }
}
