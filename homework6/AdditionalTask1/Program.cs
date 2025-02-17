using ConsoleApp1.Data;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Repository;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;
using System.Net;

class Program
{
    private static IAuthor _authors;
    private static IBook _books;
    private static ICategory _categories;
    private static IReview _reviews;
    private static IPromotion _promotions;
    private static IOrder _orders;
    public static ApplicationContext DbContext() => new ApplicationContextFactory().CreateDbContext();

    static async Task Main()
    {
        Initialize();
        string menuChoice;
        do
        {
            Console.WriteLine("1 - Authors menu\n2 - Books menu\n3 - Categories menu\n4 - Reviews menu\n5 - Promotions menu\n6 - Orders menu\n0 - Exit\n");
            menuChoice = Console.ReadLine().Trim();
            Console.Clear();

            string userInput;
            switch (menuChoice)
            {
                case "1":
                    await AuthorsMenu();
                    break;
                case "2":
                    await BooksMenu();
                    break;
                case "3":
                    await CategoriesMenu();
                    break;
                case "4":
                    await ReviewsMenu();
                    break;
                case "5":
                    await PromotionsMenu();
                    break;
                case "6":
                    await OrdersMenu();
                    break;
                case "0":
                    Console.Clear();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (menuChoice != "0");
    }

    static void Initialize()
    {
        new DbInit().Init(DbContext());
        _authors= new AuthorRepository();
        _books = new BookRepository();
        _categories = new CategoryRepository();
        _reviews = new ReviewRepository();
        _promotions = new PromotionRepository();
        _orders = new OrderRepository();
    }

    static async Task AuthorsMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Authors menu\n\n1 - Add author\n2 - Delete author\n3 - Edit author\n4 - Get all authors\n5 - Get author with books\n" +
                "6 - Get author\n7 - Get authors by name\n0 - Exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter author name:");
                    string authorName = Console.ReadLine();

                    Author newAuthor = new Author { Name = authorName };
                    await _authors.AddAuthorAsync(newAuthor);
                    break;

                case "2":
                    Console.WriteLine("Enter author ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int authorIdToDelete))
                    {
                        Author authorToDelete = await _authors.GetAuthorAsync(authorIdToDelete);
                        if (authorToDelete != null)
                            await _authors.DeleteAuthorAsync(authorToDelete);
                        else
                            Console.WriteLine("Author not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter author ID to edit:");
                    if (int.TryParse(Console.ReadLine(), out int authorIdToEdit))
                    {
                        Author authorToEdit = await _authors.GetAuthorAsync(authorIdToEdit);
                        if (authorToEdit != null)
                        {
                            Console.WriteLine("Enter new name:");
                            authorToEdit.Name = Console.ReadLine();

                            await _authors.EditAuthorAsync(authorToEdit);
                        }
                        else
                        {
                            Console.WriteLine("Author not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    var authors = await _authors.GetAllAuthorsAsync();
                    foreach (var author in authors)
                    {
                        Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
                    }
                    break;

                case "5":
                    Console.WriteLine("Enter author ID:");
                    if (int.TryParse(Console.ReadLine(), out int authorId))
                    {
                        Author author = await _authors.GetAuthorAsync(authorId);
                        if (author != null)
                            Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
                        else
                            Console.WriteLine("Author not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "6":
                    Console.WriteLine("Enter author name:");
                    string searchName = Console.ReadLine();
                    var authorsByName = await _authors.GetAuthorsByNameAsync(searchName);
                    foreach (var author in authorsByName)
                    {
                        Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
                    }
                    break;

                case "7":
                    Console.WriteLine("Enter author ID:");
                    if (int.TryParse(Console.ReadLine(), out int authorWithBooksId))
                    {
                        Author authorWithBooks = await _authors.GetAuthorWhithBooksAsync(authorWithBooksId);
                        if (authorWithBooks != null)
                        {
                            Console.WriteLine($"Name: {authorWithBooks.Name}");
                            foreach (var book in authorWithBooks.Books)
                            {
                                Console.WriteLine($"Book: {book.Title}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Author not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }

    static async Task BooksMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Books menu\n\n1 - Add book\n2 - Delete book\n3 - Edit book\n4 - Get all books\n5 - Get book by id\n" +
                "6 - Get book by name\n7 - Get book with authors\n8 - Get book with promotion\n9 - Get book with category and authors\n" +
                "10 - Get book with authors and reviews\n11 - Get book with authors, reviews, and category\n0 - Exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter book title:");
                    string title = Console.ReadLine();
                    Console.WriteLine("Enter book description:");
                    string description = Console.ReadLine();
                    Console.WriteLine("Enter published date (yyyy-mm-dd):");
                    DateTime publishedOn;
                    if (!DateTime.TryParse(Console.ReadLine(), out publishedOn))
                    {
                        Console.WriteLine("Invalid date format.");
                        break;
                    }
                    Console.WriteLine("Enter book price:");
                    decimal price;
                    if (!decimal.TryParse(Console.ReadLine(), out price))
                    {
                        Console.WriteLine("Invalid price format.");
                        break;
                    }
                    Console.WriteLine("Enter book category id:");
                    int categoryId;
                    if (!int.TryParse(Console.ReadLine(), out categoryId))
                    {
                        Console.WriteLine("Invalid category id format.");
                        break;
                    }
                    Book newBook = new Book { Title = title, Description = description, PublishedOn = publishedOn, Price = price, Authors = new List<Author>(), CategoryId = categoryId };

                    Console.WriteLine("Enter author ID (or 0 to finish):");
                    while (true)
                    {
                        if (!int.TryParse(Console.ReadLine(), out int authorId) || authorId == 0)
                            break;

                        Author author = await _authors.GetAuthorAsync(authorId);
                        if (author != null)
                            newBook.Authors.Add(author);
                        else
                            Console.WriteLine($"No author found with ID {authorId}");
                    }

                    await _books.AddBookAsync(newBook);
                    break;

                case "2":
                    Console.WriteLine("Enter book ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int bookIdToDelete))
                    {
                        Book bookToDelete = await _books.GetBookAsync(bookIdToDelete);
                        if (bookToDelete != null)
                            await _books.DeleteBookAsync(bookToDelete);
                        else
                            Console.WriteLine("Book not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter book ID to edit:");
                    if (int.TryParse(Console.ReadLine(), out int bookIdToEdit))
                    {
                        Book bookToEdit = await _books.GetBookAsync(bookIdToEdit);
                        if (bookToEdit != null)
                        {
                            Console.WriteLine("Enter new title:");
                            bookToEdit.Title = Console.ReadLine();
                            Console.WriteLine("Enter new description:");
                            bookToEdit.Description = Console.ReadLine();
                            Console.WriteLine("Enter new published date (yyyy-mm-dd):");
                            if (!DateTime.TryParse(Console.ReadLine(), out DateTime newPublishedDate))
                            {
                                Console.WriteLine("Invalid date format.");
                                break;
                            }
                            bookToEdit.PublishedOn = newPublishedDate;

                            Console.WriteLine("Enter new price:");
                            if (!decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                            {
                                Console.WriteLine("Invalid price format.");
                                break;
                            }
                            bookToEdit.Price = newPrice;

                            await _books.EditBookAsync(bookToEdit);
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    var books = await _books.GetAllBooksAsync();
                    foreach (var book in books)
                    {
                        Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Price: {book.Price}");
                    }
                    break;

                case "5":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookId))
                    {
                        Book book = await _books.GetBookAsync(bookId);
                        if (book != null)
                            Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Description: {book.Description}, Price: {book.Price}");
                        else
                            Console.WriteLine("Book not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "6":
                    Console.WriteLine("Enter book title:");
                    string searchTitle = Console.ReadLine();
                    var booksByName = await _books.GetBooksByNameAsync(searchTitle);
                    foreach (var book in booksByName)
                    {
                        Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Price: {book.Price}");
                    }
                    break;

                case "7":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookWithAuthorsId))
                    {
                        Book bookWithAuthors = await _books.GetBookWithAuthorsAsync(bookWithAuthorsId);
                        if (bookWithAuthors != null)
                        {
                            Console.WriteLine($"Title: {bookWithAuthors.Title}");
                            foreach (var author in bookWithAuthors.Authors)
                            {
                                Console.WriteLine($"Author: {author.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "8":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookWithPromoId))
                    {
                        Book bookWithPromo = await _books.GetBookWithPromotionAsync(bookWithPromoId);
                        if (bookWithPromo != null)
                        {
                            Console.WriteLine($"Title: {bookWithPromo.Title}, Promotion: {bookWithPromo.Promotion?.Percent}%");
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "9":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookWithCategoryId))
                    {
                        Book bookWithCategory = await _books.GetBookWithCategoryAndAuthorsAsync(bookWithCategoryId);
                        if (bookWithCategory != null)
                        {
                            Console.WriteLine($"Title: {bookWithCategory.Title}, Category: {bookWithCategory.Category?.Name}");
                            foreach (var author in bookWithCategory.Authors)
                            {
                                Console.WriteLine($"Author: {author.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "10":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookWithReviewsId))
                    {
                        Book bookWithReviews = await _books.GetBookWithAuthorsAndReviewAsync(bookWithReviewsId);
                        if (bookWithReviews != null)
                        {
                            Console.WriteLine($"Title: {bookWithReviews.Title}");
                            foreach (var author in bookWithReviews.Authors)
                            {
                                Console.WriteLine($"Author: {author.Name}");
                            }
                            foreach (var review in bookWithReviews.Reviews)
                            {
                                Console.WriteLine($"Review: {review.Comment}, Rating: {review.Stars}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "11":
                    Console.WriteLine("Enter book ID:");
                    if (int.TryParse(Console.ReadLine(), out int bookFullId))
                    {
                        Book bookFull = await _books.GetBooksWithAuthorsAndReviewAndCategoryAsync(bookFullId);
                        if (bookFull != null)
                        {
                            Console.WriteLine($"Title: {bookFull.Title}, Category: {bookFull.Category?.Name}");
                            foreach (var author in bookFull.Authors)
                            {
                                Console.WriteLine($"Author: {author.Name}");
                            }
                            foreach (var review in bookFull.Reviews)
                            {
                                Console.WriteLine($"Review: {review.Comment}, Rating: {review.Stars}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Book not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }

    static async Task CategoriesMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Categories menu\n\n1 - Add category\n2 - Delete category\n3 - Edit category\n4 - Get all categories\n5 - Get category by id\n6 - Get category by name\n7 - Get category with books\n0 - Exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter category name:");
                    string categoryName = Console.ReadLine();
                    Category newCategory = new Category { Name = categoryName };
                    await _categories.AddCategoryAsync(newCategory);
                    break;

                case "2":
                    Console.WriteLine("Enter category ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int categoryIdToDelete))
                    {
                        Category categoryToDelete = await _categories.GetCategoryAsync(categoryIdToDelete);
                        if (categoryToDelete != null)
                            await _categories.DeleteCategoryAsync(categoryToDelete);
                        else
                            Console.WriteLine("Category not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter category ID to edit:");
                    if (int.TryParse(Console.ReadLine(), out int categoryIdToEdit))
                    {
                        Category categoryToEdit = await _categories.GetCategoryAsync(categoryIdToEdit);
                        if (categoryToEdit != null)
                        {
                            Console.WriteLine("Enter new category name:");
                            categoryToEdit.Name = Console.ReadLine();
                            await _categories.UpdateCategoryAsync(categoryToEdit);
                        }
                        else
                        {
                            Console.WriteLine("Category not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    var categories = await _categories.GetAllCategoriesAsync();
                    foreach (var category in categories)
                    {
                        Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
                    }
                    break;

                case "5":
                    Console.WriteLine("Enter category ID:");
                    if (int.TryParse(Console.ReadLine(), out int categoryId))
                    {
                        Category category = await _categories.GetCategoryAsync(categoryId);
                        if (category != null)
                            Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
                        else
                            Console.WriteLine("Category not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "6":
                    Console.WriteLine("Enter category name:");
                    string searchName = Console.ReadLine();
                    var categoriesByName = await _categories.GetCategoriesByNameAsync(searchName);
                    foreach (var category in categoriesByName)
                    {
                        Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
                    }
                    break;

                case "7":
                    Console.WriteLine("Enter category ID:");
                    if (int.TryParse(Console.ReadLine(), out int categoryWithBooksId))
                    {
                        Category categoryWithBooks = await _categories.GetCategoryWithBooksAsync(categoryWithBooksId);
                        if (categoryWithBooks != null)
                        {
                            Console.WriteLine($"Category: {categoryWithBooks.Name}");
                            foreach (var book in categoryWithBooks.Books)
                            {
                                Console.WriteLine($"Book: {book.Title}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Category not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }
    static async Task ReviewsMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Reviews menu\n\n1 - Get all books reviews\n2 - Get review\n3 - Add review\n4 - Delete review\n0 - exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter book ID to get all reviews:");
                    if (int.TryParse(Console.ReadLine(), out int bookIdForReviews))
                    {
                        var reviews = await _reviews.GetAllReviewsAsync(bookIdForReviews);
                        foreach (var review in reviews)
                        {
                            Console.WriteLine($"ID: {review.Id}, Rating: {review.Stars}, Comment: {review.Comment}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter review ID to get:");
                    if (int.TryParse(Console.ReadLine(), out int reviewId))
                    {
                        Review review = await _reviews.GetReviewAsync(reviewId);
                        if (review != null)
                            Console.WriteLine($"ID: {review.Id}, Rating: {review.Stars}, Comment: {review.Comment}");
                        else
                            Console.WriteLine("Review not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter book ID for the review:");
                    if (int.TryParse(Console.ReadLine(), out int reviewBookId))
                    {
                        Console.WriteLine("Enter rating (1-5):");
                        byte rating = byte.Parse(Console.ReadLine());

                        Console.WriteLine("Enter comment:");
                        string comment = Console.ReadLine();

                        Review newReview = new Review { BookId = reviewBookId, Stars = rating, Comment = comment };
                        await _reviews.AddReviewAsync(newReview);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    Console.WriteLine("Enter review ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int reviewIdToDelete))
                    {
                        Review reviewToDelete = await _reviews.GetReviewAsync(reviewIdToDelete);
                        if (reviewToDelete != null)
                            await _reviews.DeleteReviewAsync(reviewToDelete);
                        else
                            Console.WriteLine("Review not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }

    static async Task PromotionsMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Promotions menu\n\n1 - Get all promotions\n2 - Get promotion\n3 - Add promotion\n4 - Edit promotion\n5 - Delete promotion\n0 - Exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter book ID to get all promotions:");
                    if (int.TryParse(Console.ReadLine(), out int bookIdForPromotions))
                    {
                        var promotions = await _promotions.GetAllPromotionsAsync();
                        foreach (var promotion in promotions)
                        {
                            if (promotion.BookId == bookIdForPromotions)
                            {
                                Console.WriteLine(promotion);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter promotion ID to get details:");
                    if (int.TryParse(Console.ReadLine(), out int promotionId))
                    {
                        var promotion = await _promotions.GetPromotionAsync(promotionId);
                        if (promotion != null)
                            Console.WriteLine(promotion);
                        else
                            Console.WriteLine("Promotion not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter book ID for the promotion:");
                    if (int.TryParse(Console.ReadLine(), out int bookIdForNewPromotion))
                    {
                        Console.WriteLine("Enter promotion name:");
                        string promotionName = Console.ReadLine();

                        Console.WriteLine("Enter discount percent or amount:");
                        decimal discount = decimal.Parse(Console.ReadLine());

                        Promotion newPromotion = new Promotion
                        {
                            BookId = bookIdForNewPromotion,
                            Name = promotionName,
                            Percent = discount >= 1 ? discount : (decimal?)null,
                            Amount = discount < 1 ? discount : (decimal?)null
                        };

                        await _promotions.AddPromotionAsync(newPromotion);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    Console.WriteLine("Enter promotion ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int promotionIdToDelete))
                    {
                        Promotion promotionToDelete = await _promotions.GetPromotionAsync(promotionIdToDelete);
                        if (promotionToDelete != null)
                            await _promotions.DeletePromotionAsync(promotionToDelete);
                        else
                            Console.WriteLine("Promotion not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }

    static async Task OrdersMenu()
    {
        string userInput;
        do
        {
            Console.WriteLine("Orders menu\n\n1 - Get all orders\n2 - Get all orders by name\n3 - Get all orders by adress\n" +
                "4 - Get order\n5 - Get order with order line and books\n6 - Add order\n7 - Update order\n8 - Delete order\n0 - Exit\n");
            userInput = Console.ReadLine().Trim();
            Console.Clear();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter customer name to get all orders:");
                    string customerName = Console.ReadLine();
                    var ordersByName = await _orders.GetAllOrdersByNameAsync(customerName);
                    foreach (var order in ordersByName)
                    {
                        Console.WriteLine($"Order ID: {order.Id}, Customer: {order.CustomerName}, Shipped: {order.Shipped}");
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter address to get all orders:");
                    string address = Console.ReadLine();
                    var ordersByAddress = await _orders.GetAllOrdersByAddressAsync(address);
                    foreach (var order in ordersByAddress)
                    {
                        Console.WriteLine($"Order ID: {order.Id}, Address: {order.Address}, Shipped: {order.Shipped}");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter order ID to get details:");
                    if (int.TryParse(Console.ReadLine(), out int orderId))
                    {
                        var order = await _orders.GetOrderWithOrderLinesAndBooksAsync(orderId);
                        if (order != null)
                        {
                            Console.WriteLine($"Order ID: {order.Id}, Customer: {order.CustomerName}, Shipped: {order.Shipped}");
                            foreach (var line in order.Lines)
                            {
                                Console.WriteLine($"Book: {line.Book.Title}, Quantity: {line.Quantity}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Order not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "4":
                    Console.WriteLine("Enter customer name:");
                    string newCustomerName = Console.ReadLine();

                    Console.WriteLine("Enter address:");
                    string newAddress = Console.ReadLine();

                    Console.WriteLine("Enter shipped status (true/false):");
                    bool shipped = bool.TryParse(Console.ReadLine(), out bool shippedStatus) ? shippedStatus : false;

                    Order newOrder = new Order
                    {
                        CustomerName = newCustomerName,
                        Address = newAddress,
                        Shipped = shipped
                    };

                    await _orders.AddOrderAsync(newOrder);
                    break;

                case "5":
                    Console.WriteLine("Enter order ID to delete:");
                    if (int.TryParse(Console.ReadLine(), out int orderIdToDelete))
                    {
                        Order orderToDelete = await _orders.GetOrderAsync(orderIdToDelete);
                        if (orderToDelete != null)
                            await _orders.DeleteOrderAsync(orderToDelete);
                        else
                            Console.WriteLine("Order not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case "0":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Invalid input.\n");
                    break;
            }

        } while (userInput != "0");
    }
}
