using ConsoleApp1.Data;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Repository;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    private static IDelivery _deliveries;
    private static IProduct _products;
    private static ICategory _categories;
    private static IUser _users;

    public static ApplicationContext DbContext() => new ApplicationContextFactory().CreateDbContext();

    static async Task Main()
    {
        Initialize();
        string menuChoice;
        do
        {
            Console.WriteLine("1 - Добавить пользователя\n2 - Добавить категорию\n3 - Добавить товар\n4 - Добавить доставку" +
                "\n5 - Просмотр категорий\n6 - Просмотр товаров\n7 - Просмотр доставок\n0 - Выход");
            menuChoice = Console.ReadLine().Trim();
            Console.Clear();

            switch (menuChoice)
            {
                case "1":
                    await AddUser();
                    break;
                case "2":
                    await AddCategory();
                    break;
                case "3":
                    await AddProduct();
                    break;
                case "4":
                    await AddDelivery();
                    break;
                case "5":
                    await ViewCategories();
                    break;
                case "6":
                    await ViewProducts();
                    break;
                case "7":
                    await ViewDeliveries();
                    break;
                case "0":
                    Console.Clear();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Неверный ввод\n");
                    break;
            }
        } while (menuChoice != "0");
    }

    static void Initialize()
    {
        _deliveries = new DeliveryRepository();
        _products = new ProductRepository();
        _categories = new CategoryRepository();
        _users = new UserRepository();
    }

    static async Task AddUser()
    {
        Console.Write("Введите email пользователя: ");
        var email = Console.ReadLine();
        Console.Write("Введите пароль пользователя: ");
        var password = Console.ReadLine();

        var user = new User
        {
            Email = email,
            Password = password
        };

        await _users.AddUserAsync(user);
        Console.WriteLine("Пользователь добавлен");
    }

    static async Task AddCategory()
    {
        Console.Write("Введите название категории: ");
        var name = Console.ReadLine();
        Console.Write("Введите описание категории: ");
        var description = Console.ReadLine();

        var category = new Category
        {
            Name = name,
            Description = description
        };

        await _categories.AddCategoryAsync(category);
        Console.WriteLine("Категория добавлена");
    }

    static async Task AddProduct()
    {
        Console.Write("Введите название товара: ");
        var name = Console.ReadLine();

        Console.Write("Введите описание товара: ");
        var description = Console.ReadLine();

        Console.Write("Введите закупочную цену товара: ");
        var purchasePrice = decimal.Parse(Console.ReadLine());

        Console.Write("Введите розничную цену товара: ");
        var retailPrice = decimal.Parse(Console.ReadLine());

        Console.Write("Введите количество товара: ");
        var quantity = int.Parse(Console.ReadLine());

        Console.Write("Введите производителя товара: ");
        var manufacturer = Console.ReadLine(); 

        Console.Write("Введите срок годности товара (yyyy-mm-dd): ");
        var expiryDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Введите категорию товара (Id): ");
        var categoryId = int.Parse(Console.ReadLine()); 

        var product = new Product
        {
            Name = name,
            Description = description,
            PurchasePrice = purchasePrice,
            RetailPrice = retailPrice,
            Quantity = quantity,
            Manufacturer = manufacturer,
            ExpiryDate = expiryDate,
            CategoryId = categoryId
        };

        await _products.AddProductAsync(product); 
        Console.WriteLine("Товар добавлен");
    }

    static async Task AddDelivery()
    {
        Console.Write("Введите ФИО получателя: ");
        var fullName = Console.ReadLine();
        Console.Write("Введите адрес доставки: ");
        var address = Console.ReadLine();
        Console.Write("Введите статус доставки: ");
        var status = Console.ReadLine();
        Console.Write("Введите тип оплаты: ");
        var paymentType = Console.ReadLine();
        Console.Write("Введите реквизиты: ");
        var requisites = Console.ReadLine();
        Console.Write("Введите время отправки заказа (yyyy-mm-dd): ");
        var shippngDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Введите время получения заказа (yyyy-mm-dd): ");
        var receivingDate = DateTime.Parse(Console.ReadLine());

        var delivery = new Delivery
        {
            FullName = fullName,
            Address = address,
            Status = status,
            PaymentType = paymentType,
            Requisites = requisites,
            ShippingDate = shippngDate,
            ReceivingDate = receivingDate
        };

        await _deliveries.AddDeliveryAsync(delivery);
        Console.WriteLine("Доставка добавлена");
    }

    static async Task ViewCategories()
    {
        var categories = await _categories.GetAllCategoriesAsync();
        foreach (var category in categories)
        {
            Console.WriteLine($"Название: {category.Name}, Описание: {category.Description}");
        }
    }

    static async Task ViewProducts()
    {
        var products = await _products.GetAllProductsAsync();
        foreach (var product in products)
        {
            Console.WriteLine($"Название: {product.Name}, Описание: {product.Description}\n Закупочная цена: " +
                $"{product.PurchasePrice}, Розничная цена: {product.RetailPrice},Количество: {product.Quantity}");
        }
    }

    static async Task ViewDeliveries()
    {
        var deliveries = await _deliveries.GetAllDeliveriesAsync();
        foreach (var delivery in deliveries)
        {
            Console.WriteLine($"ФИО: {delivery.FullName}, Адрес: {delivery.Address}, Статус: {delivery.Status}");
        }
    }
}
