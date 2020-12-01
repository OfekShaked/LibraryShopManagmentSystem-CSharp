using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace BLL
{
    public interface IBookService
    {
        Task AddNewBook(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity, string price, string isbn, string edition, string summary);
        Task UpdateBook(string idToUpdatem, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string isbn, string edition, string summary);
        Task<List<Book>> GetAllAvailableBooks();
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBook(string itemID);
        Task DeleteBook(string itemID);
        Task SellBook(string itemID,string username);

    }
}
