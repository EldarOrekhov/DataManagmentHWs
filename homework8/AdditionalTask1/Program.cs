using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var db = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;");
            await db.OpenAsync();

            var libraryService = new LibraryService(db);

            var booksByAuthor = await libraryService.GetBooksByAuthor("J.K. Rowling");
            foreach (var book in booksByAuthor)
            {
                Console.WriteLine($"{book.BookTitle} by {book.AuthorName} ({book.CategoryName}) - ${book.Price} - {book.ReleaseDate.ToShortDateString()}");
            }

            await libraryService.IncreasePriceBy5Percent();

            var authorBookCounts = await libraryService.GetBooksCountByAuthor();
            foreach (var author in authorBookCounts)
            {
                Console.WriteLine($"{author.AuthorName} has {author.BooksCount} books.");
            }
        }
    }

    public class LibraryService
    {
        private readonly IDbConnection _db;

        public LibraryService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BookInfo>> GetBooksByAuthor(string authorName)
        {
            var query = @"
                SELECT b.Title AS BookTitle, 
                       a.Name AS AuthorName, 
                       c.Name AS CategoryName, 
                       b.Price, 
                       b.ReleaseDate
                FROM Books b
                JOIN Authors a ON b.AuthorId = a.AuthorId
                JOIN Categories c ON b.CategoryId = c.CategoryId
                WHERE a.Name = @AuthorName";

            return await _db.QueryAsync<BookInfo>(query, new { AuthorName = authorName });
        }

        public async Task DeleteBookWithLowestPriceInCategory(int categoryId)
        {
            var query = @"
                DELETE FROM Books
                WHERE BookId = (
                    SELECT TOP 1 BookId
                    FROM Books
                    WHERE CategoryId = @CategoryId
                    ORDER BY Price ASC
                )";

            await _db.ExecuteAsync(query, new { CategoryId = categoryId });
        }

        public async Task IncreasePriceBy5Percent()
        {
            var query = "UPDATE Books SET Price = Price * 1.05";
            await _db.ExecuteAsync(query);
        }

        public async Task<IEnumerable<BookInfo>> GetBooksInPriceRange(decimal minPrice, decimal maxPrice)
        {
            var query = @"
                SELECT b.Title, a.Name AS AuthorName, c.Name AS CategoryName, b.Price, b.ReleaseDate
                FROM Books b
                JOIN Authors a ON b.AuthorId = a.AuthorId
                JOIN Categories c ON b.CategoryId = c.CategoryId
                WHERE b.Price BETWEEN @MinPrice AND @MaxPrice";

            return await _db.QueryAsync<BookInfo>(query, new { MinPrice = minPrice, MaxPrice = maxPrice });
        }

        public async Task<IEnumerable<AuthorBookCount>> GetBooksCountByAuthor()
        {
            var query = @"
                SELECT a.AuthorId, a.Name AS AuthorName, COUNT(b.BookId) AS BooksCount
                FROM Authors a
                LEFT JOIN Books b ON a.AuthorId = b.AuthorId
                GROUP BY a.AuthorId, a.Name";

            return await _db.QueryAsync<AuthorBookCount>(query);
        }
    }

    public class BookInfo
    {
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class AuthorBookCount
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int BooksCount { get; set; }
    }
}
