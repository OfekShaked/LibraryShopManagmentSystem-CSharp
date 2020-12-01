using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GeneralItemException:DALException
    {
        public GeneralItemException()
        {
        }

        public GeneralItemException(string message)
            : base(message)
        {
        }

        public GeneralItemException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
