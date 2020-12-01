using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DAL
{
    public interface ILibraryQueries
    {
      
        Task<List<string>> GetAllPublishers();
        Task<List<string>> GetAllAuthors();
        Task<List<string>> GetAllItemNames();
    }
}
