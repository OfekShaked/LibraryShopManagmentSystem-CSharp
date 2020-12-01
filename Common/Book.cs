using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Book : AbstractItem
    {
        public string ISBN { get; set; }
        public string Edition { get; set; }
        public string Summary { get; set; }
        public Book()
        {

        }
        public Book(string iSBN, string edition, string summary,string name, string writer, DateTime printDate, string publisher, Genre genre, int discount, int quantity, int price):base(name,writer,printDate,publisher,genre,discount,quantity,price)
        {
            this.ISBN = iSBN;
            this.Edition = edition;
            this.Summary = summary;
        }
        public override string ToString()
        {
            return $"{base.ToString()},{ISBN},{Edition},{Summary}";
        }
        public override bool Equals(object obj)
        {
            Book b1= obj as Book;
            if (b1 == null)
                return false;
            if (b1.ItemID.Equals(this.ItemID))
                return true;
            return false;
        }
    }
}
