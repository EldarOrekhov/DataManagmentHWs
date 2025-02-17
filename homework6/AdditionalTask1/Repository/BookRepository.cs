using ConsoleApp1.Data;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Repository
{
    public class BookRepository : IBook
    {
        public async Task AddBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                var authors = context.Authors.Where(e => book.Authors.Select(e => e.Id).Contains(e.Id));
                book.Authors = authors.ToList();
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Remove(book);
                await context.SaveChangesAsync();
            }
        }

        public async Task EditBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                var currentBook = await context.Books.Include(e => e.Authors).FirstOrDefaultAsync(e => e.Id == book.Id);
                if (currentBook != null)
                {
                    currentBook.Title = book.Title;
                    currentBook.Description = book.Description;
                    currentBook.PublishedOn = book.PublishedOn;
                    currentBook.Price = book.Price;
                    currentBook.Authors = new List<Author>();

                    var authorsIds = book.Authors.Select(e => e.Id);
                    currentBook.Authors = await context.Authors.Where(e => authorsIds.Contains(e.Id)).ToListAsync();


                    context.Books.Update(currentBook);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.ToListAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).ToListAsync();
            }
        }

        public async Task<Book> GetBookAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByNameAsync(string name)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Where(e => e.Title.Contains(name)).ToListAsync();
            }
        }

        public async Task<Book> GetBookWithAuthorsAndReviewAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).Include(e => e.Reviews).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Book> GetBookWithAuthorsAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Book> GetBookWithCategoryAndAuthorsAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Category).Include(e => e.Authors).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Book> GetBookWithPromotionAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Promotion).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Book> GetBooksWithAuthorsAndReviewAndCategoryAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Category).Include(e => e.Authors).Include(e => e.Reviews).FirstOrDefaultAsync(e => e.Id == id);
            }
        }
    }
}
