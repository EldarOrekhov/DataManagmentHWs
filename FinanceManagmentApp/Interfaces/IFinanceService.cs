using FinanceManagmentApp.Models;

namespace FinanceManagmentApp.Interfaces
{
    public interface IFinanceService
    {
        Task<decimal> GetTotalIncomeAsync(int userId);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
    }
}
