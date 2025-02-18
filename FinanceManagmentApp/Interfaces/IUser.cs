using FinanceManagmentApp.Models;

namespace FinanceManagmentApp.Interfaces
{
    public interface IUser
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
