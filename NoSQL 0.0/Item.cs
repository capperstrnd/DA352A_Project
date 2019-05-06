using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class Item
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public Boolean BonusItem { get; set; }
        public int Quantity { get; set; }

        public Item(string name, int price, int quantity, bool bonusItem)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            BonusItem = bonusItem;
        }
    }
}
