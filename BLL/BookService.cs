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
    public class BookService : IBookService
    {
        IBookRepository _bookRep;
        IGenreRepository _genreRep;
        ISaleService _saleService;
        public BookService(IBookRepository bookRep, IGenreRepository genreRep,ISaleService saleService)
        {
            _genreRep = genreRep;
            _bookRep = bookRep;
            _saleService = saleService;
        }
        public async Task AddNewBook(string name, string writer, string printdate, string publisher, string genre, string discount, string quantity, string price, string isbn, string edition, string summary)
        {
            Book b1 = new Book();
            if (name == "" || writer == "" || printdate == "" || printdate == "" || publisher == "" || genre == "" || discount == "" || quantity == "" || price == "" || isbn == "" || edition == "" || summary == "")
            {
                await GeneralLibraryLogic.SaveToLogFile("All book fields must be full in order to add a new book!");
                throw new BLLBookException("All book fields must be full in order to add a new book!");
            }
            b1.Name = name;
            b1.Writer = writer;
            DateTime date;
            if(DateTime.TryParse(printdate,out date)==false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Cannot Convert date to DateTime!");
                throw new BLLBookException("Date Time Not in a correct format");
            }
            b1.PrintDate = DateTime.Parse(printdate);
            b1.Publisher = publisher;
            try
            {
                Genre g1 = await _genreRep.GetGenre(genre);
                if (g1.Name == null)
                {
                    g1.Name = genre;
                    await _genreRep.AddGenre(g1);
                    g1 = await _genreRep.GetGenre(genre);
                }
                b1.Genre = g1;
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                   await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot add a new book atm try again later");
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
                await GeneralLibraryLogic.SaveToLogFile("discount is not a number!");
                throw new BLLBookException("discount is not a number!");
            }
            if (discountToAdd < 0 || discountToAdd > 99)
            {
                await GeneralLibraryLogic.SaveToLogFile("Discount must be between 0-99");
                throw new BLLBookException("Discount must be between 0-99");
            }
            b1.Discount = discountToAdd;
            int quantityToAdd = 0;
            if (int.TryParse(quantity, out quantityToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("quantity is not a number");
                throw new BLLBookException("quantity is not a number");
            }
            if (quantityToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Quantity cannot be negative!");
                throw new BLLBookException("Quantity cannot be negative!");
            }
            b1.Quantity = quantityToAdd;
            int priceToAdd = 0;
            if (int.TryParse(price, out priceToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price is not a number");
                throw new BLLBookException("Price is not a number");
            }
            if (priceToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price cannot be negative!");
                throw new BLLBookException("Price cannot be negative!");
            }
            b1.Price = priceToAdd;
            try
            {
                List<string> isbns = await _bookRep.GetAllIsbn();
                if (isbns.Contains(isbn))
                {
                    await GeneralLibraryLogic.SaveToLogFile("ISBN Already Exist!");
                    throw new BLLBookException("ISBN Already Exist!");
                }
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot add a new book atm try again later");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            b1.ISBN = isbn;
            b1.Edition = edition;
            b1.Summary = summary;
            try
            {
                await _bookRep.AddBook(b1);
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot add a new book atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task DeleteBook(string itemID)
        {
            int id = 0;
            if (int.TryParse(itemID, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid cannot convert to int");
                throw new BLLBookException("Id is not valid");
            }
            try
            {
                Book b1 = await _bookRep.GetBook(id);
                await _bookRep.DeleteBook(b1);
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot delete a book atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Get a list of all the books with quantity of more than 1
        /// </summary>
        /// <returns></returns>
        public async Task<List<Book>> GetAllAvailableBooks()
        {
            try
            {
                List<Book> books = await _bookRep.GetAllBooks();
                for (int i = 0; i < books.Count; i++)
                {
                    if (books[i].Quantity == 0)
                    {
                        books.RemoveAt(i);
                    }
                }
                return books;
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException
                     || e is ArgumentOutOfRangeException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot get all available books atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                return await _bookRep.GetAllBooks();
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot get all books atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public async Task<Book> GetBook(string itemID)
        {
            int id = 0;
            if (int.TryParse(itemID, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid");
                throw new BLLBookException("Id is not valid");
            }
            try
            {
                return await _bookRep.GetBook(id);
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot get a book atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        /// <summary>
        /// Remove 1 from the quantity of books if book quantity is 1 or more
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public async Task SellBook(string itemID,string username)
        {
            int id;
            if(int.TryParse(itemID,out id)==false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Wrong id cannot convert to int");
                throw new BLLBookException("Cannot sell a book id is not correct");
            }
            Book b1 = await GetBook(itemID);
            if (b1.Quantity > 0)
            {
                b1.Quantity -= 1;
            }
            else
            {
                await GeneralLibraryLogic.SaveToLogFile("Cannot sell a book out of stock");
                throw new BLLBookException("Cannot sell a book out of stock");
            }
            try
            {
                await UpdateBook(itemID, b1.Name, b1.Writer, b1.PrintDate.ToString(), b1.Publisher, b1.Genre.Name, b1.Discount.ToString(), b1.Quantity.ToString(),b1.Price.ToString(), b1.ISBN, b1.Edition, b1.Summary);
                await _saleService.AddNewSale(username, id, 1);
            }
            catch (Exception e)
            {
                if (e is BookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot sell a book atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
        public async Task UpdateBook(string idToUpdate, string name, string writer, string printdate, string publisher, string genre, string discount, string quantity, string price, string isbn, string edition, string summary)
        {
            if (idToUpdate == "" || name == "" || writer == "" || printdate == "" || printdate == "" || publisher == "" || genre == "" || discount == "" || quantity == "" || price == "" || isbn == "" || edition == "" || summary == "")

            {
                await GeneralLibraryLogic.SaveToLogFile("All book fields must be full!");
                throw new BLLBookException("All book fields must be full!");
            }
            int id = 0;
            if (int.TryParse(idToUpdate, out id) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Id is not valid");
                throw new BLLBookException("Id is not valid");
            }
            Book b1 = new Book();
            b1.Name = name;
            b1.Writer = writer;
            DateTime date;
            if(DateTime.TryParse(printdate,out date)==false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Print Date is not in a correct format");
                throw new BLLBookException("Print Date is not in a correct format");
            }
            b1.PrintDate = DateTime.Parse(printdate);
            b1.Publisher = publisher;
            try
            {
                Genre g1 = await _genreRep.GetGenre(genre);
                if (g1.Name == null)
                {
                    g1.Name = genre;
                    await _genreRep.AddGenre(g1);
                }
                b1.Genre = g1;
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot sell a book atm try again later or call a manager");
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
                await GeneralLibraryLogic.SaveToLogFile("discount is not a number! cannot convert to number");
                throw new BLLBookException("discount is not a number!");
            }
            if (discountToAdd < 0 || discountToAdd > 99)
            {
                await GeneralLibraryLogic.SaveToLogFile("Discount must be between 0-99");
                throw new BLLBookException("Discount must be between 0-99");
            }
            b1.Discount = discountToAdd;
            int quantityToAdd = 0;
            if (int.TryParse(quantity, out quantityToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("quantity is not a number");
                throw new BLLBookException("quantity is not a number");
            }
            if (quantityToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Quantity cannot be negative!");
                throw new BLLBookException("Quantity cannot be negative!");
            }
            b1.Quantity = quantityToAdd;
            int priceToAdd = 0;
            if (int.TryParse(price, out priceToAdd) == false)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price is not a number!");
                throw new BLLBookException("Price is not a number!");
            }
            if (priceToAdd < 0)
            {
                await GeneralLibraryLogic.SaveToLogFile("Price cannot be negative!");
                throw new BLLBookException("Price cannot be negative!");
            }
            b1.Price = priceToAdd;
            b1.ISBN = isbn;
            b1.Edition = edition;
            b1.Summary = summary;
            try
            {
                await _bookRep.UpdateBook(id, b1);
            }
            catch (Exception e)
            {
                if (e is BLLBookException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLBookException("Cannot update a book atm try again later or call a manager");
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
