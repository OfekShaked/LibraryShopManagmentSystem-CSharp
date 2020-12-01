using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LibraryException : Exception
    {
        public LibraryException()
        {
        }

        public LibraryException(string message)
            : base(message)
        {
        }

        public LibraryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
