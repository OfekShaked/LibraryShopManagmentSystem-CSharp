using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface ISaleService
    {
        Task AddNewSale(string salesManUsername, int itemSoldID, int quantitySold);
    }
}
