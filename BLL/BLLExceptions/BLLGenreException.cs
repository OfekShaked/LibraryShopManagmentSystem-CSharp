using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLGenreException : LibraryException
    {
        public BLLGenreException()
        {
        }

        public BLLGenreException(string message)
            : base(message)
        {
        }

        public BLLGenreException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
