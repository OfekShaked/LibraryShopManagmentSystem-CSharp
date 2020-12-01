using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string username);
        Task AddUser(User b1);
        Task DeleteUser(User b1);
        Task UpdateUser(string username, User updatedUser);
    }
}
