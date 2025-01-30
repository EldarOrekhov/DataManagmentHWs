using Microsoft.EntityFrameworkCore;
using ConsoleApp1;

namespace OrderServiceClass;

public class OrderService
{
    private readonly ApplicationContext dbContext;

    public OrderService(ApplicationContext db) { dbContext = db; }

    public void AddOrder(List<Product> products, string phone)
    {
        Order order = new Order {Phone=phone , Products = products };
        dbContext.Orders.Add(order);
        dbContext.SaveChanges();
        Console.WriteLine($"Order {order.Id} succes");
    }

    public void DeleteOrder(int orderId)
    {
        Order order = dbContext.Orders.Include(o => o.Products).FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            dbContext.Orders.Remove(order);
            dbContext.SaveChanges();
            Console.WriteLine($"Order {order.Id} deleted");
        }
        else
            Console.WriteLine($"Order not found");
    }

    public void ShowOrders()
    {
        List<Order> orders = dbContext.Orders.Include(o => o.Products).ToList();
        foreach (Order order in orders)
        {
            Console.WriteLine($"Order {order.Id} for {order.Phone} by ({order.OrderTime}): " + string.Join(", ", order.Products.Select(p => p.Name)));
        }
    }
}