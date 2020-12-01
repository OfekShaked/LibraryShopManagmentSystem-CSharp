using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Journal:AbstractItem
    {
        public string Subject { get; set; }
        public Journal()
        {

        }
        public Journal(string subject, string name, string writer, DateTime printDate, string publisher, Genre genre, int discount, int quantity,int price) : base(name, writer, printDate, publisher, genre, discount, quantity,price)
        {
            Subject = subject;
        }
        public override string ToString()
        {
            return $"{base.ToString()},{Subject}";
        }
        public override bool Equals(object obj)
        {
            Journal b1 = obj as Journal;
            if (b1 == null)
                return false;
            if (b1.ItemID.Equals(this.ItemID))
                return true;
            return false;
        }
    }
}
