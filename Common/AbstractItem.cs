using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class AbstractItem
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Writer { get; set; }
        public DateTime PrintDate { get; set; }
        public string Publisher { get; set; }
        public Genre Genre { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }

        public AbstractItem(string name, string writer, DateTime printDate, string publisher, Genre genre, int discount, int quantity, int price)
        {
            this.Name = name;
            this.Writer = writer;
            this.PrintDate = printDate;
            this.Publisher = publisher;
            this.Genre = genre;
            this.Discount = discount;
            this.Quantity = quantity;
            this.Price = price;
        }
        public AbstractItem()
        {

        }
        public override string ToString()
        {
            return $"{ItemID},{Name},{Writer},{PrintDate},{Publisher},{Genre.Name},{Discount},{Quantity},{Price}";
        }
    }
}
