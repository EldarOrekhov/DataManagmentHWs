using FinanceManagmentApp.Data;
using FinanceManagmentApp.Interfaces;
using FinanceManagmentApp.Models;
using FinanceManagmentApp.Repositories;
using FinanceManagmentApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace FinanceManagmentApp
{
    class Program
    {
        private static ITransaction _transactionRepository;
        private static IUser _userRepository;
        private static IFinanceService _financeService;
        private static ApplicationContext _context;

        static async Task Main()
        {
            Initialize();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Меню:\n" +
                                  "1 - Добавить транзакцию\n" +
                                  "2 - Просмотреть список транзакций\n" +
                                  "3 - Получить общий доход\n" +
                                  "4 - Получить общий расход\n" +
                                  "5 - Получить общий баланс\n" +
                                  "6 - Фильтровать транзакции\n" +
                                  "7 - Отчет о состоянии финансов\n" +
                                  "8 - Добавить пользователя\n" +
                                  "0 - Выйти");
                Console.Write("Введите номер: ");

                var choice = Console.ReadLine();

                if (choice == "0")
                    break;

                switch (choice)
                {
                    case "1":
                        await AddTransaction();
                        break;
                    case "2":
                        await ViewTransactions();
                        break;
                    case "3":
                        await GetTotalIncome();
                        break;
                    case "4":
                        await GetTotalExpenses();
                        break;
                    case "5":
                        await GetBalance();
                        break;
                    case "6":
                        await FilterTransactions();
                        break;
                    case "7":
                        await FinancialReport();
                        break;
                    case "8":
                        await AddUser();
                        break;
                    default:
                        Console.WriteLine("Неверный ввод, попробуйте снова.");
                        break;
                }
            }
        }

        static async Task AddTransaction()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            Console.Write("Введите тип транзакции (1 - доход, 2 - расход): ");
            var type = Console.ReadLine();

            Console.Write("Введите сумму: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Неверный формат суммы.");
                return;
            }

            Console.Write("Введите описание: ");
            string description = Console.ReadLine();

            var transaction = new Transaction
            {
                UserId = userId,
                Type = type == "1" ? TransactionType.Income : TransactionType.Expense,
                Amount = amount,
                Description = description,
                Date = DateTime.Now,
                CategoryId = 1
            };

            await _transactionRepository.AddTransactionAsync(transaction);
            Console.WriteLine("Транзакция добавлена");
        }

        static async Task ViewTransactions()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            var transactions = await _transactionRepository.GetTransactionsByUserAsync(userId);
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"{transaction.Date} - {transaction.Type} - {transaction.Amount} - {transaction.Description}");
            }
        }

        static async Task GetTotalIncome()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            var totalIncome = await _financeService.GetTotalIncomeAsync(userId);
            Console.WriteLine($"Общий доход: {totalIncome}");
        }

        static async Task GetTotalExpenses()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            var totalExpenses = await _financeService.GetTotalExpensesAsync(userId);
            Console.WriteLine($"Общий расход: {totalExpenses}");
        }

        static async Task GetBalance()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            var balance = await _financeService.GetBalanceAsync(userId);
            Console.WriteLine($"Общий баланс: {balance}");
        }

        static async Task FilterTransactions()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            Console.Write("Введите тип транзакции (1 - доход, 2 - расход): ");
            var type = Console.ReadLine();
            var transactionType = type == "1" ? TransactionType.Income : TransactionType.Expense;

            Console.Write("Введите начальную дату (yyyy-MM-dd): ");
            var startDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Введите конечную дату (yyyy-MM-dd): ");
            var endDate = DateTime.Parse(Console.ReadLine());

            var transactions = await _transactionRepository.GetTransactionsByTypeAndDateAsync(userId, transactionType, startDate, endDate);
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"{transaction.Date} - {transaction.Type} - {transaction.Amount} - {transaction.Description}");
            }
        }

        static async Task FinancialReport()
        {
            Console.Write("Введите ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный формат ID пользователя");
                return;
            }

            var totalIncome = await _financeService.GetTotalIncomeAsync(userId);
            var totalExpenses = await _financeService.GetTotalExpensesAsync(userId);
            var balance = totalIncome - totalExpenses;

            Console.WriteLine($"Отчет по состоянию финансов:");
            Console.WriteLine($"Общий доход: {totalIncome}");
            Console.WriteLine($"Общий расход: {totalExpenses}");
            Console.WriteLine($"Баланс: {balance}");
        }

        static async Task AddUser()
        {
            Console.Write("Введите имя пользователя: ");
            string name = Console.ReadLine();

            var user = new User { Name = name };
            await _userRepository.AddUserAsync(user);

            Console.WriteLine($"Пользователь {name} добавлен ID: {user.Id}");
        }

        static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer(connectionString);

            _context = new ApplicationContext(optionsBuilder.Options);

            _transactionRepository = new TransactionRepository(_context);
            _userRepository = new UserRepository(_context);

            _financeService = new FinanceService(_transactionRepository, _userRepository);
        }
    }
}
