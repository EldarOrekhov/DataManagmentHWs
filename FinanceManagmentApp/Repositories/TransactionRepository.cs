using FinanceManagmentApp.Data;
using FinanceManagmentApp.Interfaces;
using FinanceManagmentApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagmentApp.Repositories
{
    public class TransactionRepository : ITransaction
    {
        private readonly ApplicationContext _context;

        public TransactionRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalTransactionAmountByUserAsync(int userId)
        {
            var query = "SELECT SUM(Amount) FROM Transactions WHERE UserId = {0}";
            var totalAmount = await _context.Database.ExecuteSqlRawAsync(query, userId);
            return totalAmount;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCategory(int categoryId)
        {
            var transactions = await _context.Transactions
                .FromSqlRaw("EXEC GetTransactionsByCategory @CategoryId = {0}", categoryId)
                .ToListAsync();
            return transactions;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.Include(t => t.User).Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.Include(t => t.User).Include(t => t.Category).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId).Include(t => t.Category).ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await GetTransactionByIdAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAndDateAsync(int userId, TransactionType transactionType, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.Type == transactionType &&
                            t.Date >= startDate &&
                            t.Date <= endDate)
                .Include(t => t.Category)
                .ToListAsync();
        }

    }
}
