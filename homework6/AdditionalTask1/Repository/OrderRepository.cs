using ConsoleApp1.Data;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Repository
{
    public class OrderRepository : IOrder
    {
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Orders.ToListAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersByNameAsync(string name)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Orders.Where(e => e.CustomerName.Contains(name)).ToListAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersByAddressAsync(string address)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Orders.Where(e => e.Address.Contains(address)).ToListAsync();
            }
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Orders.FirstOrDefaultAsync(e => e.Id == id); 
            }
        }

        public async Task<Order> GetOrderWithOrderLinesAndBooksAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Orders.Include(e => e.Lines).ThenInclude(e => e.Book).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Orders.Update(order);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOrderAsync(Order order)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
        }
    }
}
