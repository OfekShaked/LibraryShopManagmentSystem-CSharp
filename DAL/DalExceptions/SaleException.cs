using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    class SaleException:DALException
    {
        public SaleException()
        {
        }

        public SaleException(string message)
            : base(message)
        {
        }

        public SaleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
