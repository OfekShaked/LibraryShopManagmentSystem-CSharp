using Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BookRepository : IBookRepository
    {
        IGenreRepository _genreRep;
        public BookRepository(IGenreRepository genreRep)
        {
            _genreRep = genreRep;
        }
        public async Task<List<Book>> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string GetBooksQuery = "SELECT ItemID, Name, Writer, PrintDate, Publisher, IDOfGenre, Discount, Quantity,Price, Isbn, Edition, Summary " +
                                   "FROM AbstractItems " +
                                   "WHERE Subject is null";
            try
            {
                SqliteCommand command = new SqliteCommand(GetBooksQuery);
                List<string> booksString = SqlFileAccess.GetData(command);
                for (int i = 0; i < booksString.Count; i++)
                {
                    books.Add(await TextToBook(booksString[i]));
                }
                return books;
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant get all books");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
                         
        }
        /// <summary>
        /// Get Book By Id
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>Book Found or null if not found</returns>
        public async Task<Book> GetBook(int itemID)
        {
            List<Book> books = new List<Book>();
            string GetBooksQuery = "SELECT ItemID, Name, Writer, PrintDate, Publisher, IDOfGenre, Discount, Quantity,Price, Isbn, Edition, Summary " +
                                   "FROM AbstractItems " +
                                   $"WHERE ItemID = @id AND Subject IS NULL";
            try
            {
                SqliteCommand command = new SqliteCommand(GetBooksQuery);
                command.Parameters.AddWithValue("@id", itemID);
                List<string> booksString = SqlFileAccess.GetData(command);
                if (booksString.Count == 0)
                    return null;
                books.Add(await TextToBook(booksString[0]));
                return books[0];
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant get book");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        public async Task AddBook(Book b1)
        {
            string insertQuery = "INSERT INTO AbstractItems(Name,Writer,PrintDate,Publisher,IDOfGenre,Discount,Quantity,Price,Isbn,Edition,Summary)" +
                                $"VALUES(@name,@writer,@printDate,@publisher,@genreID,@discount,@quantity,@price,@isbn,@edition,@summary)";
            try
            {
                SqliteCommand command = new SqliteCommand(insertQuery);
                command.Parameters.AddWithValue("@name", b1.Name);
                command.Parameters.AddWithValue("@writer", b1.Writer);
                command.Parameters.AddWithValue("@printDate", b1.PrintDate);
                command.Parameters.AddWithValue("@publisher", b1.Publisher);
                command.Parameters.AddWithValue("@genreID", b1.Genre.ID);
                command.Parameters.AddWithValue("@discount", b1.Discount);
                command.Parameters.AddWithValue("@quantity", b1.Quantity);
                command.Parameters.AddWithValue("@price", b1.Price);
                command.Parameters.AddWithValue("@isbn", b1.ISBN);
                command.Parameters.AddWithValue("@edition", b1.Edition);
                command.Parameters.AddWithValue("@summary", b1.Summary);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant add book");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        public async Task DeleteBook(Book b1)
        {
            string deleteQuery = $"DELETE FROM AbstractItems WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(deleteQuery);
                command.Parameters.AddWithValue("@id", b1.ItemID);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant Delete book");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Update a book by giving the book id and the new book data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedBook"></param>
        /// <returns></returns>
        public async Task UpdateBook(int id, Book updatedBook)
        {
            string updateQuery = "UPDATE AbstractItems " +
                                 $"Set Name = @name , Writer = @writer , PrintDate = @printDate , Publisher = @publisher , " +
                                 $"IDOfGenre = @genreID , Discount = @discount , Quantity = @quantity , Price = @price , Isbn = @isbn , " +
                                 $"Edition = @edition , Summary = @summary "+
                                 $"WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(updateQuery);
                command.Parameters.AddWithValue("@name", updatedBook.Name);
                command.Parameters.AddWithValue("@writer", updatedBook.Writer);
                command.Parameters.AddWithValue("@printDate", updatedBook.PrintDate);
                command.Parameters.AddWithValue("@publisher", updatedBook.Publisher);
                command.Parameters.AddWithValue("@genreID", updatedBook.Genre.ID);
                command.Parameters.AddWithValue("@discount", updatedBook.Discount);
                command.Parameters.AddWithValue("@quantity", updatedBook.Quantity);
                command.Parameters.AddWithValue("@price", updatedBook.Price);
                command.Parameters.AddWithValue("@isbn", updatedBook.ISBN);
                command.Parameters.AddWithValue("@edition", updatedBook.Edition);
                command.Parameters.AddWithValue("@summary", updatedBook.Summary);
                command.Parameters.AddWithValue("@id", id);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant Update book");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Returns a list of all the isbns exist
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllIsbn()
        {
            string getAllIsbnQuery = "SELECT Isbn FROM AbstractItems WHERE Isbn is not null ";
            try
            {
                SqliteCommand command = new SqliteCommand(getAllIsbnQuery);
                return SqlFileAccess.GetData(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant get isbns");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        private async Task<Book> TextToBook(string text)
        {
            try
            {
                string[] bookProps = text.Split(',');
                Book book = new Book();
                book.ItemID = int.Parse(bookProps[0]);
                book.Name = bookProps[1];
                book.Writer = bookProps[2];
                book.PrintDate = DateTime.Parse(bookProps[3]);
                book.Publisher = bookProps[4];
                book.Genre = await _genreRep.GetGenreByID(int.Parse(bookProps[5]));
                book.Discount = int.Parse(bookProps[6]);
                book.Quantity = int.Parse(bookProps[7]);
                book.Price = int.Parse(bookProps[8]);
                book.ISBN = bookProps[9];
                book.Edition = bookProps[10];
                book.Summary = bookProps[11];
                return book;
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is GenreException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Cant get Genre");
                }
                else if(e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new BookException("Book Writing Error");
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
