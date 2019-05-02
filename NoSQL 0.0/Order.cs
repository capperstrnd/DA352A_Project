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
        public Customer Costumer { get; set; }
        public Employee Employee { get; set; }
        public List<Item> Items { get; set; }
        public int TotalCost { get; set; }

        public Order(Customer costumer, Employee employee, List<Item> items, int totalCost)
        {
            Costumer = costumer;
            Employee = employee;
            Items = items;
            TotalCost = totalCost;
        }
    }
}
