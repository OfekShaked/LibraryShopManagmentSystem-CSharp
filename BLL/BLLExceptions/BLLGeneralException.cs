using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLGeneralException:LibraryException
    {
        public BLLGeneralException()
        {
        }

        public BLLGeneralException(string message)
            : base(message)
        {
        }

        public BLLGeneralException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
