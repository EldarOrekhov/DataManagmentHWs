using FinanceManagmentApp.Interfaces;
using FinanceManagmentApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagmentApp.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ITransaction _transactionRepository;
        private readonly IUser _userRepository;

        public FinanceService(ITransaction transactionRepository, IUser userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task<decimal> GetTotalIncomeAsync(int userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByUserAsync(userId);
            if (transactions == null || !transactions.Any())
                return 0; 

            return transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpensesAsync(int userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByUserAsync(userId);
            if (transactions == null || !transactions.Any())
                return 0; 

            return transactions
                .Where(t => t.Type == TransactionType.Expense) 
                .Sum(t => t.Amount); 
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var income = await GetTotalIncomeAsync(userId);
            var expenses = await GetTotalExpensesAsync(userId);
            return income - expenses;
        }
    }
}
