using Common;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class JournalService : IJournalService
    {
        IJournalRepository _journalRep;
        IGenreRepository _genreRep;
        ISaleService _saleService;
        public JournalService(IJournalRepository journalRep, IGenreRepository genreRep, ISaleService saleService)
        {
            _genreRep = genreRep;
            _journalRep = journalRep;
            _saleService = saleService;
        }
        public async Task AddNewJournal(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string subject)
        {
            if (name == "" || writer == "" || printdate == "" || publisher == "" || genre == "" || discount == "" || quantity == "" || price == "" || subject == "")
            {
                await GeneralLibraryLogic.SaveToLogFile("All journal fields must be full");
                throw new BLLJournalException("All journal fields must be full");
            }
            Journal j1 = new Journal();
            j1.Name = name;
            j1.Writer = writer;
            DateTime date;
            if(DateTime.TryParse(printdate,out date)==false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Cannot convert printdate To DateTime!");
                throw new BLLJournalException("Cannot convert printdate To DateTime!");
            }
            j1.PrintDate = date;
            j1.Publisher = publisher;
            try
            {
                Genre g1 = await _genreRep.GetGenre(genre);
                if (g1.Name == null)
                {
                    g1.Name = genre;
                    await _genreRep.AddGenre(g1);
                    g1 = await _genreRep.GetGenre(genre);
                }
                j1.Genre = g1;

            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot add a new journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }

            int discountToAdd = 0;
            if (int.TryParse(discount, out discountToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Discount must be a number!");
                throw new BLLJournalException("Discount must be a number!");
            }
            if (discountToAdd < 0 || discountToAdd > 99)
            {
                await GeneralLibraryLogic.SaveToLogFile("Discount must be between 0-99");
                throw new BLLJournalException("Discount must be between 0-99");
            }
            j1.Discount = discountToAdd;
            int quantityToAdd = 0;
            if (int.TryParse(quantity, out quantityToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Quantity must be a number!");
                throw new BLLJournalException("Quantity must be a number!");
            }
            if (quantityToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Quantity cannot be negative!");
                throw new BLLJournalException("Quantity cannot be negative!");
            }
            j1.Quantity = quantityToAdd;
            int priceToAdd = 0;
            if (int.TryParse(price, out priceToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price must be a number!");
                throw new LibraryException("Price must be a number!");
            }
            if (priceToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price cannot be negative!");
                throw new LibraryException("Price cannot be negative!");
            }
            j1.Price = priceToAdd;
            j1.Subject = subject;
            try
            {
                await _journalRep.AddJournal(j1);
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot add a new journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task DeleteJournal(string itemID)
        {
            int id = 0;
            if (int.TryParse(itemID, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid");
                throw new BLLJournalException("Id is not valid");
            }
            try
            {
                Journal j1 = await _journalRep.GetJournal(id);
                await _journalRep.DeleteJournal(j1);
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot delete a journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Get all journals where quantity is more than 0.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Journal>> GetAllAvailableJournals()
        {
            try
            {
                List<Journal> journals = await _journalRep.GetAllJournals();
                for (int i = 0; i < journals.Count; i++)
                {
                    if (journals[i].Quantity == 0)
                    {
                        journals.RemoveAt(i);
                    }
                }
                return journals;
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot get all available journals atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task<List<Journal>> GetAllJournals()
        {
            try
            {
                return await _journalRep.GetAllJournals();
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot get all journals atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Get a journal by journal id
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>Journal found or null</returns>
        public async Task<Journal> GetJournal(string itemID)
        {
            int id = 0;
            if (int.TryParse(itemID, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid cannot convert to int");
                throw new BLLJournalException("Id is not valid");
            }
            try
            {
                return await _journalRep.GetJournal(id);
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot get a journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task UpdateJournal(string idToUpdate, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string subject)
        {
            if (idToUpdate == "" || name == "" || writer == "" || printdate == "" || publisher == "" || genre == "" || discount == "" || quantity == "" || price == "" || subject == "")
            {
                await GeneralLibraryLogic.SaveToLogFile("All journal fields must be full");
                throw new BLLJournalException("All journal fields must be full");
            }
            int id = 0;
            if (int.TryParse(idToUpdate, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid cannot convert to int");
                throw new BLLJournalException("Id is not valid");
            }
            Journal j1 = new Journal();
            j1.ItemID = id;
            j1.Name = name;
            j1.Writer = writer;
            DateTime date;
            if(DateTime.TryParse(printdate,out date)==false)
            {
                await GeneralLibraryLogic.SaveToLogFile("print date is not in a valid format!");
                throw new BLLJournalException("print date is not in a valid format!");
            }
            j1.PrintDate = date;
            j1.Publisher = publisher;
            try
            {
                Genre g1 = await _genreRep.GetGenre(genre);
                if (g1.Name == null)
                {
                    g1.Name = genre;
                    await _genreRep.AddGenre(g1);
                }
                j1.Genre = g1;
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot update a journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            int discountToAdd = 0;
            if (int.TryParse(discount, out discountToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("discount is not a number");
                throw new BLLJournalException("discount is not a number");
            }
            if (discountToAdd < 0 || discountToAdd > 99)
            {
                await GeneralLibraryLogic.SaveToLogFile("Discount must be between 0-99");
                throw new BLLJournalException("Discount must be between 0-99");
            }
            j1.Discount = discountToAdd;
            int quantityToAdd = 0;
            if (int.TryParse(quantity, out quantityToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("quantity is not a number");
                throw new BLLJournalException("quantity is not a number");
            }
            if (quantityToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Quantity cannot be negative!");
                throw new BLLJournalException("Quantity cannot be negative!");
            }
            j1.Quantity = quantityToAdd;
            int priceToAdd = 0;
            if (int.TryParse(price, out priceToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price must be a number!");
                throw new BLLJournalException("Price must be a number!");
            }
            if(priceToAdd<0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price cannot be negative!");
                throw new BLLJournalException("Price cannot be negative!");
            }
            j1.Price = priceToAdd;
            j1.Subject = subject;
            try
            {
                await _journalRep.UpdateJournal(id, j1);
            }
            catch (Exception e)
            {
                if (e is JournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot update a journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Removes 1 from the quantity of the journal if the quantity is 1 or more
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public async Task SellJournal(string itemID,string username)
        {
            int id;
            if (int.TryParse(itemID, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("ID is not valid cannot convert to int!");
                throw new BLLJournalException("ID is not valid cannot convert to int!");
            }
            try
            {
                Journal j1 = await GetJournal(itemID);
                if (j1.Quantity > 0)
                {
                    j1.Quantity -= 1;
                }
                await UpdateJournal(itemID, j1.Name, j1.Writer, j1.PrintDate.ToString(), j1.Publisher, j1.Genre.Name, j1.Discount.ToString(), j1.Quantity.ToString(),j1.Price.ToString(), j1.Subject);
                await _saleService.AddNewSale(username, id, 1);
            }
            catch (Exception e)
            {
                if (e is BLLJournalException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLJournalException("Cannot sell a journal atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

    }
}
