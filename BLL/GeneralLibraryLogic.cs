using Common;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BLL
{
    public class GeneralLibraryLogic : IGeneralLibraryLogic
    {
        IUserValidity _userValidity;
        IBookService _bookService;
        IJournalService _journalService;
        IGenereService _genereService;
        ILibraryQueries _libraryQueries;
        IUserRepository _userRep;
        IGeneralRepository _generalRep;
        IGenreRepository _genreRep;
        IBookRepository _bookRep;
        IJournalRepository _journalRep;
        ISaleRepository _saleRep;
        ISaleService _saleService;
        public GeneralLibraryLogic()
        {
            SQLiteCreation.InitializeDatabase();
            _userRep = new UserRepository();
            _saleRep = new SaleRepository();
            _saleService = new SaleService(_saleRep);
            _genreRep = new GenreRepository();
            _generalRep = new GeneralRepository(_genreRep);
            _bookRep = new BookRepository(_genreRep);
            _journalRep = new JournalRepository(_genreRep);
            _userValidity = new UserValidity(_userRep, _generalRep);
            _bookService = new BookService(_bookRep, _genreRep, _saleService);
            _journalService = new JournalService(_journalRep, _genreRep, _saleService);
            _genereService = new GenreService(_genreRep, _generalRep);
            _libraryQueries = new LibraryQueries(_bookRep, _journalRep);
        }
        public GeneralLibraryLogic(IUserRepository userRep, IGenreRepository genreRep, IGeneralRepository generalRep,IBookRepository bookRep, IJournalRepository journalRep, ISaleRepository saleRep)
        {
            _generalRep = generalRep;
            _userRep = userRep;
            _generalRep = generalRep;
            _genreRep = genreRep;
            _bookRep = bookRep;
            _journalRep = journalRep;
            _saleRep = saleRep;
            _saleService = new SaleService(_saleRep);
            _userValidity = new UserValidity(_userRep, _generalRep);
            _bookService = new BookService(_bookRep, _genreRep, _saleService);
            _journalService = new JournalService(_journalRep, _genreRep, _saleService);
            _genereService = new GenreService(_genreRep, _generalRep);
            _libraryQueries = new LibraryQueries(_bookRep, _journalRep);
        }
        /// <summary>
        /// Add the amount of discount for all the items in the list, bigger discount counts.
        /// </summary>
        /// <param name="itemsToUpdate"></param>
        /// <param name="discountToAdd"></param>
        /// <returns></returns>
        public async Task AddDiscounts(List<AbstractItem> itemsToUpdate, string discountToAdd)
        {
            int discount=0;
            if (int.TryParse(discountToAdd, out discount) == false)
            {
                await SaveToLogFile("Discount must be a number!");
                throw new BLLGeneralException("Discount must be a number!");
            }
            if (discount > 99 || discount < 0)
            {
                await SaveToLogFile("Discount must be between 0-99!");
                throw new BLLGeneralException("Discount must be between 0-99!");
            }

            try
            {
                for (int i = 0; i < itemsToUpdate.Count; i++)
                {
                    if (itemsToUpdate[i].Discount > discount)
                        discount = itemsToUpdate[i].Discount;
                    AbstractItem itemToUpdate = await _generalRep.GetItemByID(itemsToUpdate[i].ItemID);
                    itemToUpdate.Discount = discount;
                    if (itemsToUpdate != null)
                    {
                        await _generalRep.UpdateAbstractItem(itemsToUpdate[i].ItemID,itemToUpdate);
                    }
                   
                }
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot add discounts atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }

        }

        public async static Task SaveToLogFile(string error)
        {
            List<string> errors = new List<string>();
            errors.Add(error + "\n" + DateTime.Now + "\n\n");
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            if (await folder.TryGetItemAsync($"{FileNames.BLLLog.ToString()}.txt") == null)
            {
                file = await folder.CreateFileAsync($"{FileNames.BLLLog.ToString()}.txt", CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                file = await folder.GetFileAsync($"{FileNames.BLLLog.ToString()}.txt");
            }
            await FileIO.AppendLinesAsync(file, errors);
        }
        /// <summary>
        /// Get a list of all the genres starting with the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<List<string>> GetGenreSuggestions(string text)
        {
            List<string> suggestion = new List<string>();
            try
            {
                suggestion = await GetAllGenreNames();
                suggestion = suggestion.Distinct().ToList();
                suggestion = suggestion.Where(x => x.ToLower().StartsWith(text.ToLower())).ToList();
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot get genre suggestions atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return suggestion;
        }
        /// <summary>
        /// Get a list of publishers starting with the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPublisherSuggestions(string text)
        {
            List<string> suggestion = new List<string>();
            try
            {
                suggestion = await GetAllPublisherNames();
                suggestion = suggestion.Distinct().ToList();
                suggestion = suggestion.Where(x => x.ToLower().StartsWith(text.ToLower())).ToList();
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot get publisher suggestions atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return suggestion;
        }
        /// <summary>
        /// Get a list of all writers starting with the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAuthorSuggestions(string text)
        {
            List<string> suggestion = new List<string>();
            try
            {
                suggestion = await GetAllAuthorNames();
                suggestion = suggestion.Distinct().ToList();
                suggestion = suggestion.Where(x => x.ToLower().StartsWith(text.ToLower())).ToList();
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot get author suggestions atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return suggestion;
        }
        /// <summary>
        /// Get all item names starting with the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<List<string>> GetNameSuggestions(string text)
        {
            List<string> suggestion = new List<string>();
            try
            {
                suggestion = await GetAllItemNames();
                suggestion = suggestion.Distinct().ToList();
                suggestion = suggestion.Where(x => x.ToLower().StartsWith(text.ToLower())).ToList();
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot get name suggestions atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return suggestion;

        }
        /// <summary>
        /// Search for items by given parameters, if a a paramerter is empty it will not search by the parameter.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="biggerOrSmallerYear"></param>
        /// <param name="publisher"></param>
        /// <param name="writer"></param>
        /// <param name="genre"></param>
        /// <param name="discount"></param>
        /// <param name="biggerOrSmallerDisc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<AbstractItem>> SearchItems(string year, BiggerSmaller biggerOrSmallerYear, string publisher, string writer, string genre, string discount, BiggerSmaller biggerOrSmallerDisc, string name)
        {
            List<AbstractItem> matchingItems = new List<AbstractItem>();
            bool isStarted = false;
            try
            {
                if (year != "")
                {
                    matchingItems = GetAllByYear(int.Parse(year), biggerOrSmallerYear, await GetAllItems());
                    isStarted = true;
                }
                if (publisher != "")
                {
                    if (isStarted == false)
                    {
                        matchingItems = GetAllByPublisher(publisher, await GetAllItems());
                        isStarted = true;
                    }
                    else
                    {
                        matchingItems = GetAllByPublisher(publisher, matchingItems);
                    }
                }
                if (writer != "")
                {
                    if (isStarted == false)
                    {
                        matchingItems = GetAllByAuther(writer, await GetAllItems());
                        isStarted = true;
                    }
                    else
                    {
                        matchingItems = GetAllByAuther(writer, matchingItems);
                    }
                }
                if (genre != "")
                {
                    if (isStarted == false)
                    {
                        Genre g1 = await _genereService.GetGenre(genre);
                        matchingItems = GetAllByGenre(g1, await GetAllItems());
                        isStarted = true;
                    }
                    else
                    {
                        Genre g1 = await _genereService.GetGenre(genre);
                        matchingItems = GetAllByGenre(g1, matchingItems);
                    }
                }
                if (discount != "")
                {
                    if (isStarted == false)
                    {
                        matchingItems = GetAllByDiscount(int.Parse(discount), biggerOrSmallerDisc, await GetAllItems());
                        isStarted = true;
                    }
                    else
                    {
                        matchingItems = GetAllByDiscount(int.Parse(discount), biggerOrSmallerDisc, matchingItems);
                    }
                }
                if (name != "")
                {
                    if (isStarted == false)
                    {
                        matchingItems = GetAllByName(name, await GetAllItems());
                        isStarted = true;
                    }
                    else
                    {
                        matchingItems = GetAllByName(name, matchingItems);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is GeneralItemException
                     || e is DALException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new BLLGeneralException("Cannot perform a search atm try again later or call a manager");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return matchingItems;
        }

        public async Task<List<string>> GetAllPublisherNames()
        {
            return await _libraryQueries.GetAllPublishers();
        }

        public async Task<List<string>> GetAllItemNames()
        {
            return await _libraryQueries.GetAllItemNames();
        }

        public async Task<List<string>> GetAllAuthorNames()
        {
            return await _libraryQueries.GetAllAuthors();
        }

        public async Task AddNewBook(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string isbn, string edition, string summary)
        {
             await _bookService.AddNewBook(name, writer, printdate, publisher, genre, discount, quantity,price, isbn, edition, summary);
        }

        public async Task AddNewGenre(string name)
        {
            await _genereService.AddNewGenre(name);
        }

        public async Task AddNewJournal(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string subject)
        {
             await _journalService.AddNewJournal(name, writer, printdate, publisher, genre, discount, quantity,price, subject);
        }

        public async Task AddNewUser(string username, string password, string fullName, string phoneNumber, UserType type)
        {
             await _userValidity.AddNewUser(username, password, fullName, phoneNumber, type);
        }

        public async Task<bool> CheckGenre(string name)
        {
            return await _genereService.CheckGenre(name);
        }

        public async Task DeleteBook(string itemID)
        {
            await _bookService.DeleteBook(itemID);
        }

        public async Task DeleteGenre(string name)
        {
            await _genereService.DeleteGenre(name);
        }

        public async Task DeleteJournal(string itemID)
        {
            await _journalService.DeleteJournal(itemID);
        }

        public async Task<List<Book>> GetAllAvailableBooks()
        {
            return await _bookService.GetAllAvailableBooks();
        }

        public async Task<List<Journal>> GetAllAvailableJournals()
        {
            return await _journalService.GetAllAvailableJournals();
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookService.GetAllBooks();
        }

        public async Task<List<string>> GetAllGenreNames()
        {
            return await _genereService.GetAllGenreNames();
        }

        public async Task<List<Journal>> GetAllJournals()
        {
            return await _journalService.GetAllJournals();
        }

        public async Task<Book> GetBook(string itemID)
        {
            return await _bookService.GetBook(itemID);
        }

        public async Task<Genre> GetGenre(string name)
        {
            return await _genereService.GetGenre(name);
        }

        public async Task<Journal> GetJournal(string itemID)
        {
            return await _journalService.GetJournal(itemID);
        }

        public async Task UpdateBook(string idToUpdatem, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string isbn, string edition, string summary)
        {
            await _bookService.UpdateBook(idToUpdatem, name, writer, printdate, publisher, genre, discount, quantity,price, isbn, edition, summary);
        }

        public async Task UpdateGenre(string name, string newName)
        {
            await _genereService.UpdateGenre(name, newName);
        }

        public async Task UpdateJournal(string idToUpdatem, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity,string price, string subject)
        {
            await _journalService.UpdateJournal(idToUpdatem, name, writer, printdate, publisher, genre, discount, quantity,price, subject);
        }

        public async Task<UserType> VerifyUser(string username, string password)
        {
            return await _userValidity.VerifyUser(username, password);
        }
        public async Task SellBook(string itemID,string username)
        {
            await _bookService.SellBook(itemID,username);
        }
        public async Task SellJournal(string itemID,string username)
        {
            await _journalService.SellJournal(itemID,username);
        }
        public async Task SellItem(string itemID,string username)
        {
            Book b1 = await GetBook(itemID);
            Journal j1 = await GetJournal(itemID);
            if(b1!=null&&b1.Quantity>0)
            {
                await SellBook(itemID,username);
            }
            else if(j1!=null&&j1.Quantity>0)
            {
                await SellJournal(itemID, username);
            }
           
        }
        private List<AbstractItem> GetAllByDiscount(int discount, BiggerSmaller biggerOrSmaller, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            if (biggerOrSmaller == BiggerSmaller.Bigger)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Discount > discount)
                        requiredList.Add(items[i]);
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Discount < discount)
                        requiredList.Add(items[i]);
                }
            }
            return requiredList;
        }

        private List<AbstractItem> GetAllByGenre(Genre genre, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Genre.Equals(genre) == true)
                {
                    requiredList.Add(items[i]);
                }
            }
            return requiredList;
        }

        private List<AbstractItem> GetAllByName(string itemName, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Name.Equals(itemName) == true)
                {
                    requiredList.Add(items[i]);
                }
            }
            return requiredList;
        }

        private List<AbstractItem> GetAllByPublisher(string publisherName, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Publisher.Equals(publisherName) == true)
                {
                    requiredList.Add(items[i]);
                }
            }
            return requiredList;
        }
        private List<AbstractItem> GetAllByAuther(string WriterName, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Writer.Equals(WriterName) == true)
                {
                    requiredList.Add(items[i]);
                }
            }
            return requiredList;
        }

        private List<AbstractItem> GetAllByYear(int year, BiggerSmaller biggerOrSmaller, List<AbstractItem> listToGetFrom)
        {
            List<AbstractItem> items = listToGetFrom;
            List<AbstractItem> requiredList = new List<AbstractItem>();
            if (biggerOrSmaller == BiggerSmaller.Bigger)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].PrintDate.Year > year)
                    {
                        requiredList.Add(items[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].PrintDate.Year < year)
                    {
                        requiredList.Add(items[i]);
                    }
                }
            }
            return requiredList;
        }

        private async Task<List<AbstractItem>> GetAllItems()
        {
            List<AbstractItem> items = new List<AbstractItem>();
            List<Book> books = await _bookRep.GetAllBooks();
            for (int i = 0; i < books.Count; i++)
            {
                items.Add(books[i]);
            }
            List<Journal> journals = await _journalRep.GetAllJournals();
            for (int i = 0; i < journals.Count; i++)
            {
                items.Add(journals[i]);
            }
            return items;
        }
    }
}
