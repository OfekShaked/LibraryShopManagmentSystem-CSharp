using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLJournalException:LibraryException
    {
        public BLLJournalException()
        {
        }

        public BLLJournalException(string message)
            : base(message)
        {
        }

        public BLLJournalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
