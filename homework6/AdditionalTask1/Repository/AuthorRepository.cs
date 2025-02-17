using ConsoleApp1.Data;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Repository
{
    public class AuthorRepository : IAuthor
    {
        public async Task AddAuthorAsync(Author author)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                await context.Authors.AddAsync(author);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAuthorAsync(Author author)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Authors.Remove(author);
                await context.SaveChangesAsync();
            }
        }

        public async Task EditAuthorAsync(Author author)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Authors.Update(author);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Authors.ToListAsync();
            }
        }

        public async Task<Author> GetAuthorWhithBooksAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Authors.Include(e => e.Books).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Author> GetAuthorAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Authors.FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<IEnumerable<Author>> GetAuthorsByNameAsync(string name)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Authors.Where(e => e.Name.Contains(name)).ToListAsync();
            }
        }
    }
}
