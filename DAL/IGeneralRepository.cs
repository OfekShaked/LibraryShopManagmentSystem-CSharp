using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IGeneralRepository
    {
        Task<AbstractItem> GetItemByID(int id);
        Task UpdateAbstractItem(int id, AbstractItem updatedItem);



    }
}
