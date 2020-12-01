using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GenreException:DALException
    {
        public GenreException()
        {
        }

        public GenreException(string message)
            : base(message)
        {
        }

        public GenreException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
