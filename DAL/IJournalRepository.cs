using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IJournalRepository
    {
        Task<List<Journal>> GetAllJournals();
        Task<Journal> GetJournal(int id);
        Task AddJournal(Journal b1);
        Task DeleteJournal(Journal b1);
        Task UpdateJournal(int id, Journal updatedJournal);
    }
}
