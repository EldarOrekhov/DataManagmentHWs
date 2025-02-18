using FinanceManagmentApp.Models;

namespace FinanceManagmentApp.Interfaces
{
    public interface ITransaction
    {
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(int userId);
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAndDateAsync(int userId, TransactionType transactionType, DateTime startDate, DateTime endDate);
    }
}
