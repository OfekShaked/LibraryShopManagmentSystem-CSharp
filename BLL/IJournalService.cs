using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IJournalService
    {
        Task AddNewJournal(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string subject);
        Task UpdateJournal(string idToUpdatem, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price,string subject);
        Task<List<Journal>> GetAllAvailableJournals();
        Task<List<Journal>> GetAllJournals();
        Task<Journal> GetJournal(string itemID);
        Task DeleteJournal(string itemID);
        Task SellJournal(string itemID,string username);
    }
}
