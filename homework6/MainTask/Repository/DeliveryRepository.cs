using ConsoleApp1.Data;
using ConsoleApp1.Models;
using ConsoleApp1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Repository
{
    public class DeliveryRepository : IDelivery
    {
        private readonly ApplicationContext _context = Program.DbContext();

        public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
        {
            return await _context.Deliveries.ToListAsync();
        }

        public async Task<Delivery> GetDeliveryByIdAsync(int id)
        {
            return await _context.Deliveries.FindAsync(id);
        }

        public async Task AddDeliveryAsync(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeliveryAsync(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }
        }
    }
}
