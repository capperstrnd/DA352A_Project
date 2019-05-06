using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class StockLog
    {
        public ObjectId Id;
        public string Date;
        public List<Item> Items;
    }
}
