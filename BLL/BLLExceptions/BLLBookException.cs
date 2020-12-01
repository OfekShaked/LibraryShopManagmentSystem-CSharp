using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLBookException:LibraryException
    {
        public BLLBookException()
        {
        }

        public BLLBookException(string message)
            : base(message)
        {
        }

        public BLLBookException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
