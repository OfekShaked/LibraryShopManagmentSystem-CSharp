using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IGeneralLibraryLogic:IBookService,IGenereService,IJournalService,IUserValidity
    {
        Task<List<string>> GetGenreSuggestions(string text);
        Task<List<string>> GetNameSuggestions(string text);
        Task<List<string>> GetAuthorSuggestions(string text);
        Task<List<string>> GetPublisherSuggestions(string text);
        Task<List<string>> GetAllPublisherNames();
        Task<List<string>> GetAllAuthorNames();
        Task<List<string>> GetAllItemNames();
        Task SellItem(string itemID, string username);
        Task AddDiscounts(List<AbstractItem> itemsToUpdate, string discount);
        Task<List<AbstractItem>> SearchItems(string year, BiggerSmaller biggerOrSmallerYear, string publisher,string writer, string genre, string discount, BiggerSmaller biggerOrSmallerDisc, string name);
    
    }
}
