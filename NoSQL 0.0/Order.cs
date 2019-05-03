using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class Order
    {
        public ObjectId Id { get; set; }
        public ObjectId CostumerId { get; set; }
        public ObjectId EmployeeId { get; set; }
        public List<Item> Items { get; set; }
        public string Date { get; set; }
        public string City { get; set; }
        public int TotalCost { get; set; }

        public Order(ObjectId costumerId, ObjectId employeeId, List<Item> items, string date, string City, int totalCost)
        {
            CostumerId = costumerId;
            EmployeeId = employeeId;
            Items = items;
            TotalCost = totalCost;
        }
    }
}
