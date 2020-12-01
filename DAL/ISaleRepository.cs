using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface ISaleRepository
    {
        Task AddNewSale(Sale s1);
    }
}
