using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class StockItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }

        public StockItem(string name, int quantity, string city, DateTime date)
        {
            Name = name;
            Quantity = quantity;
            City = city;
            Date = date;
        }
    }
}
