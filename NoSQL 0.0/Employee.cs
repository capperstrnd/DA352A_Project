using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    
    class Employee
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string SSN { get; set; }
        public string Position { get; set; }
        public string City { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int WorkingCapacity { get; set; }
        public List<string> Comments { get; set; }

        public Employee(string name, string password, string sSN, string position, string city, string startDate, string endDate, int workingCapacity)
        {
            Name = name;
            Password = password;
            SSN = sSN;
            Position = position;
            City = city;
            StartDate = startDate;
            EndDate = endDate;
            WorkingCapacity = workingCapacity;
        }
    }
}
