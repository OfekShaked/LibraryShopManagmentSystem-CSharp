using Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class JournalRepository : IJournalRepository
    {
        IGenreRepository _genreRep;
        public JournalRepository( IGenreRepository genreRepository)
        {
            _genreRep = genreRepository;
        }
        public async Task AddJournal(Journal j1)
        {
            string insertQuery = "INSERT INTO AbstractItems(Name,Writer,PrintDate,Publisher,IDOfGenre,Discount,Quantity,Price,Subject)" +
                                 $"VALUES(@name , @writer , @printDate , @publisher , @genreID , @discount , @quantity , @price , @subject)";
            try
            {
                SqliteCommand command = new SqliteCommand(insertQuery);
                command.Parameters.AddWithValue("@name", j1.Name);
                command.Parameters.AddWithValue("@writer", j1.Writer);
                command.Parameters.AddWithValue("@printDate", j1.PrintDate);
                command.Parameters.AddWithValue("@publisher", j1.Publisher);
                command.Parameters.AddWithValue("@genreID", j1.Genre.ID);
                command.Parameters.AddWithValue("@discount", j1.Discount);
                command.Parameters.AddWithValue("@quantity", j1.Quantity);
                command.Parameters.AddWithValue("@price", j1.Price);
                command.Parameters.AddWithValue("@subject", j1.Subject);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant add journal");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        public async Task DeleteJournal(Journal j1)
        {
            string deleteQuery = $"DELETE FROM AbstractItems WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(deleteQuery);
                command.Parameters.AddWithValue("@id", j1.ItemID);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant delete journal");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        public async Task<List<Journal>> GetAllJournals()
        {
            List<Journal> journals = new List<Journal>();
            string GetJournalsQuery = "SELECT ItemID, Name, Writer, PrintDate, Publisher, IDOfGenre, Discount, Quantity,Price, Subject " +
                                   "FROM AbstractItems " +
                                   "WHERE Isbn is null";
            try
            {
                SqliteCommand command = new SqliteCommand(GetJournalsQuery);
                List<string> journalsString = SqlFileAccess.GetData(command);
                for (int i = 0; i < journalsString.Count; i++)
                {
                    journals.Add(await TextToJournal(journalsString[i]));
                }
                return journals;
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant get all journals");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Get a journal by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Journal if found, if not found returns null</returns>
        public async Task<Journal> GetJournal(int id)
        {
            List<Journal> journals = new List<Journal>();
            string GetBooksQuery = "SELECT ItemID, Name, Writer, PrintDate, Publisher, IDOfGenre, Discount, Quantity,Price,Subject " +
                                   "FROM AbstractItems " +
                                   $"WHERE ItemID = @id AND ISBN IS NULL";
            try
            {
                SqliteCommand command = new SqliteCommand(GetBooksQuery);
                command.Parameters.AddWithValue("@id", id);
                List<string> journalsString = SqlFileAccess.GetData(command);
                if (journalsString.Count == 0)
                    return null;
                journals.Add(await TextToJournal(journalsString[0]));
                return journals[0];
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant get journal");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        public async Task UpdateJournal(int id, Journal updatedJournal)
        {
            string updateQuery = "UPDATE AbstractItems " +
                                 $"Set Name = @name , Writer = @writer , PrintDate = @printDate , Publisher = @publisher , " +
                                 $"IDOfGenre = @genreID , Discount = @discount , Quantity = @quantity , Price = @price , Subject = @subject " +
                                 $"WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(updateQuery);
                command.Parameters.AddWithValue("@name", updatedJournal.Name);
                command.Parameters.AddWithValue("@writer", updatedJournal.Writer);
                command.Parameters.AddWithValue("@printDate", updatedJournal.PrintDate);
                command.Parameters.AddWithValue("@publisher", updatedJournal.Publisher);
                command.Parameters.AddWithValue("@genreID", updatedJournal.Genre.ID);
                command.Parameters.AddWithValue("@discount", updatedJournal.Discount);
                command.Parameters.AddWithValue("@quantity", updatedJournal.Quantity);
                command.Parameters.AddWithValue("@price", updatedJournal.Price);
                command.Parameters.AddWithValue("@subject", updatedJournal.Subject);
                command.Parameters.AddWithValue("@id", id);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant update journal");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        private async Task<Journal> TextToJournal(string text)
        {
            try
            {
                string[] journalProps = text.Split(',');
                Journal journal = new Journal();
                journal.ItemID = int.Parse(journalProps[0]);
                journal.Name = journalProps[1];
                journal.Writer = journalProps[2];
                journal.PrintDate = DateTime.Parse(journalProps[3]);
                journal.Publisher = journalProps[4];
                journal.Genre = await _genreRep.GetGenreByID(int.Parse(journalProps[5]));
                journal.Discount = int.Parse(journalProps[6]);
                journal.Quantity = int.Parse(journalProps[7]);
                journal.Price = int.Parse(journalProps[8]);
                journal.Subject = journalProps[9];
                return journal;
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is GenreException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Cant get Genre");
                }
                else if (e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new JournalException("Journal Writing Error");
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
