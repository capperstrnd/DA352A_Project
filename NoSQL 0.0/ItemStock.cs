using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class ItemStock
    {
        public ObjectId Id { get; set; }
        public string City { get; set; }
        public List<Item> Items { get; set; }

        public ItemStock(string city, List<Item> items)
        {
            City = city;
            Items = items;
        }
    }
}
