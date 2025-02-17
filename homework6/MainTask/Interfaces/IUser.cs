using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Interfaces
{
    public interface IUser
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
