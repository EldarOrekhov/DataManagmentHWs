using ConsoleApp1.Models;

namespace ConsoleApp1.Interfaces
{
    public interface IBook
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync();

        Task<Book> GetBookAsync(int id);
        Task<IEnumerable<Book>> GetBooksByNameAsync(string name);
        Task<Book> GetBookWithPromotionAsync(int id);
        Task<Book> GetBookWithAuthorsAsync(int id);
        Task<Book> GetBookWithCategoryAndAuthorsAsync(int id);
        Task<Book> GetBookWithAuthorsAndReviewAsync(int id);
        Task<Book> GetBooksWithAuthorsAndReviewAndCategoryAsync(int id);

        Task AddBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task EditBookAsync(Book book);
    }
}
