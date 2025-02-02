using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1;
class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //for (int i = 1; i < 12; i++) 
            //    db.Books.Add(new Book { Name = $"Book{i}", Author = $"Author{i}" });
            //db.SaveChanges();

            AuthService authService = new AuthService();
            BookService bookService = new BookService();
            User currentUser = null;
            string choice;
            do
            {
                if (currentUser == null)
                {
                    Console.Write("Enter 1 to Register, 2 to Login, 0 to Exit: ");
                    choice = Console.ReadLine().Trim();
                    string username, password;
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Enter login: ");
                            username = Console.ReadLine();
                            Console.Write("Enter password: ");
                            password = Console.ReadLine();
                            authService.Register(username, password);
                            break;
                        case "2":
                            Console.Write("Enter login: ");
                            username = Console.ReadLine();
                            Console.Write("Enter password: ");
                            password = Console.ReadLine();
                            currentUser = authService.Login(username, password);
                            break;
                        case "0":
                            break;
                        default:
                            Console.WriteLine("Wrong input");
                            break;
                    }   
                }
                else
                {
                    Console.Write("\nEnter 1 to View all books, 2 to Search book, 3 to Enter paginate, 0 to Exit: ");
                    choice = Console.ReadLine().Trim();
                    switch (choice)
                    {
                        case "1":
                            bookService.ViewBooks(); 
                            break;
                        case "2":
                            Console.WriteLine("Enter book name: ");
                            string bookName = Console.ReadLine();
                            bookService.SearchBook(bookName);
                            break;
                        case "3":
                            bookService.PaginateBooks();
                            break;
                        case "0":
                            break;
                        default:
                            Console.WriteLine("Wrong input");
                            break;
                    }
                }
            } while (choice != "0");
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int FailedAttempts { get; set; }
    public bool IsLocked { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
}

public class AuthService
{
    public void Register(string username, string password)
    {
        using (var db = new ApplicationContext())
        {
            if (db.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already taken");
                return;
            }
            string hash = new PasswordHasher().HashPassword(password);
            db.Users.Add(new User { Username = username, PasswordHash = hash });
            db.SaveChanges();
            Console.WriteLine($"User added successfuly: {username}");
        }
    }

    public User Login(string username, string password)
    {
        using (var db = new ApplicationContext())
        {
            User? user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }
            else if (user.IsLocked)
            {
                Console.WriteLine("User locked");
                return null;
            }
            else if (new PasswordHasher().VerifyPassword(password, user.PasswordHash))
            {
                Console.WriteLine("Success login");
                user.FailedAttempts = 0;
                db.SaveChanges();
                return user;
            }
            else
            {
                user.FailedAttempts++;
                Console.WriteLine("Wrong password");
                if (user.FailedAttempts >= 3)
                {
                    user.IsLocked = true;
                    Console.WriteLine("User locked due to unsuccessful login attempts");
                }
                db.SaveChanges();
                return null;
            }
        }
    }
}

public class BookService
{
    public void ViewBooks()
    {
        using (var db = new ApplicationContext())
        {
            Console.Clear();
            List<Book> books = db.Books.ToList();
            if (books.Count > 0)
            {
                foreach (Book book in books)
                {
                    Console.WriteLine($"{book.Name} by {book.Author}");
                }
            }
            else
                Console.WriteLine("No books"); 
        }
    }

    public void SearchBook(string title)
    {
        using (var db = new ApplicationContext())
        {
            Console.Clear();
            var books = db.Books.Where(b => b.Name.Contains(title)).ToList();
            if (books.Count > 0)
            {
                foreach (Book book in books)
                {
                    Console.WriteLine($"{book.Name} by {book.Author}");
                }
            }
            else
                Console.WriteLine("No books found");
        }
    }

    public void PaginateBooks()
    {
        int page = 0;
        string input;
        do
        {
            using (var db = new ApplicationContext())
            {
                Console.Clear();
                int booksCount = db.Books.Count();
                int maxPage = booksCount / 5;
                if (booksCount % 5 != 0) maxPage++;

                var books = db.Books.Skip(page * 5).Take(5).ToList();
                foreach (var book in books)
                {
                    Console.WriteLine($"{book.Name} by {book.Author}");
                }
                Console.WriteLine($"Current page: {page+1}");

                if (page == 0)
                    Console.WriteLine("\nNext page - 2, Quit - 0: ");   
                else if (page+1 == maxPage)
                    Console.WriteLine("\nPrevious page - 1, Quit - 0: ");
                else
                    Console.WriteLine("\nPrevious page - 1, Next page - 2, Quit - 0: ");

                input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        if (page > 0) page--;
                        break;
                    case "2":
                        if (page+1 < maxPage) page++;
                        break;
                    case "0":
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        } while (input != "0");
    }
}

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(e => e.FailedAttempts).HasDefaultValue(0);
            e.Property(e => e.IsLocked).HasDefaultValue(false);
        });

        base.OnModelCreating(modelBuilder);
    }
}

public class PasswordHasher 
{ 
    public string HashPassword(string password) 
    { 
        string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        return passwordHash; 
    } 
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}