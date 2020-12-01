using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace BLL
{
    public interface IUserValidity
    {
        Task<UserType> VerifyUser(string username,string password);
        Task AddNewUser(string username, string password, string fullName, string phoneNumber, UserType type);
    }
}
