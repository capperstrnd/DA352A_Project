﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class StockLog
    {
        public ObjectId Id { get; set; }
        public string Date { get; set; }
        public string City { get; set; }
        public ItemStock CurrentStock { get; set; }

        public StockLog(string date, string city, ItemStock currentStock)
        {
            Date = date;
            City = city;
            CurrentStock = currentStock;
        }
    }
}
