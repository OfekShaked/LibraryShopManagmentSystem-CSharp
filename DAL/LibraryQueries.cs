using Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class LibraryQueries : ILibraryQueries
    {
        IBookRepository _bookRep;
        IJournalRepository _journalRep;
        public LibraryQueries(IBookRepository bookRep, IJournalRepository journalRep)
        {
            _bookRep = bookRep;
            _journalRep = journalRep;
        }
        /// <summary>
        /// Get a list of all writer names
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllAuthors()
        {
            string getAllAuthorsQuery = "SELECT Writer FROM AbstractItems";
            try
            {
                SqliteCommand command = new SqliteCommand(getAllAuthorsQuery);
                return SqlFileAccess.GetData(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new QueryException("Cant get all authors");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        
        /// <summary>
        /// Get a list of all publisher names
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllPublishers()
        {
            string getAllPublishersQuery = "SELECT Publisher FROM AbstractItems";
            try
            {
                SqliteCommand command = new SqliteCommand(getAllPublishersQuery);
                return SqlFileAccess.GetData(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new QueryException("Cant get all authors");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        public async Task<List<string>> GetAllItemNames()
        {
            string getAllNamesQuery = "SELECT Name FROM AbstractItems";
            try
            {
                SqliteCommand command = new SqliteCommand(getAllNamesQuery);
                return SqlFileAccess.GetData(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new QueryException("Cant get all authors");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

    }
}
