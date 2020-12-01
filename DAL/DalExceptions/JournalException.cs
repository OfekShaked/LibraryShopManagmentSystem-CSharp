using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class JournalException : DALException
    {
        public JournalException()
        {
        }

        public JournalException(string message)
            : base(message)
        {
        }

        public JournalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
