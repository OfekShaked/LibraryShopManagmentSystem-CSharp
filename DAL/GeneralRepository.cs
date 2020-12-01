using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Data.Sqlite;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace DAL
{
    public class GeneralRepository : IGeneralRepository
    {
        IGenreRepository _genreRep;
        public GeneralRepository(IGenreRepository genreRep)
        {
            _genreRep = genreRep;
        }
        
        public static async Task SaveToLogFile(string error)
        {
            List<string> errors = new List<string>();
            errors.Add(error + "\n" + DateTime.Now + "\n\n");
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            if (await folder.TryGetItemAsync($"{FileNames.DALLog.ToString()}.txt") == null)
            {
                file = await folder.CreateFileAsync($"{FileNames.DALLog.ToString()}.txt", CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                file = await folder.GetFileAsync($"{FileNames.DALLog.ToString()}.txt");
            }
            await FileIO.AppendLinesAsync(file, errors);
        }
        /// <summary>
        /// Get a general item by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AbstractItem> GetItemByID(int id)
        {
            List<AbstractItem> items = new List<AbstractItem>();
            string GetItemsQuery = "SELECT ItemID, Name, Writer, PrintDate, Publisher, IDOfGenre, Discount, Quantity,Price " +
                                   "FROM AbstractItems " +
                                   $"WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(GetItemsQuery);
                command.Parameters.AddWithValue("@id", id);
                List<string> itemsString = SqlFileAccess.GetData(command);
                if (itemsString.Count == 0)
                    return null;
                items.Add(await TextToItem(itemsString[0]));
                return items[0];
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new GeneralItemException("Cant get Item By ID");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }

        }
        /// <summary>
        /// Updated general item data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedItem"></param>
        /// <returns></returns>
        public async Task UpdateAbstractItem(int id, AbstractItem updatedItem)
        {
            string updateQuery = "UPDATE AbstractItems " +
                                 $"Set Name = @name , Writer = @writer , PrintDate = @printDate , Publisher = @publisher , " +
                                 $"IDOfGenre = @genreID , Discount = @discount , Quantity = @quantity , Price = @price " +
                                 $"WHERE ItemID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(updateQuery);
                command.Parameters.AddWithValue("@name", updatedItem.Name);
                command.Parameters.AddWithValue("@writer", updatedItem.Writer);
                command.Parameters.AddWithValue("@printDate", updatedItem.PrintDate);
                command.Parameters.AddWithValue("@publisher", updatedItem.Publisher);
                command.Parameters.AddWithValue("@genreID", updatedItem.Genre.ID);
                command.Parameters.AddWithValue("@discount", updatedItem.Discount);
                command.Parameters.AddWithValue("@quantity", updatedItem.Quantity);
                command.Parameters.AddWithValue("@price", updatedItem.Price);
                command.Parameters.AddWithValue("@id", id);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new GeneralItemException("Cant get Update Abstract Item");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        private async Task<AbstractItem> TextToItem(string text)
        {
            try
            {
                string[] itemProps = text.Split(',');
                AbstractItem item = new Book();
                item.ItemID = int.Parse(itemProps[0]);
                item.Name = itemProps[1];
                item.Writer = itemProps[2];
                item.PrintDate = DateTime.Parse(itemProps[3]);
                item.Publisher = itemProps[4];
                item.Genre = await _genreRep.GetGenreByID(int.Parse(itemProps[5]));
                item.Discount = int.Parse(itemProps[6]);
                item.Quantity = int.Parse(itemProps[7]);
                item.Price = int.Parse(itemProps[8]);
                return item;
            }
            catch (Exception e)
            {
                if (e is SqliteException||e is GenreException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new GeneralItemException("Get Genre By ID");
                }
                else if(e is ArgumentOutOfRangeException)
                {
                    await SaveToLogFile(e.ToString());
                    throw new DALException("Cant Convert Text To Item");
                }
                else
                {
                    await SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }

        }
    }
}
