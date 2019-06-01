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
        private string name;
        public string Name
        {
            get { return name; }
            set { if (value.Length > 0) name = value; else throw new Exception("Name must be filled."); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { if (value.Length > 0) password = value; else throw new Exception("Password must be filled."); }
        }
        private string ssn;
        public string SSN
        {
            get { return ssn; }
            set
            {
                if (value.Length != 11 || !value.Contains('-'))
                    throw new Exception("SSN is not correct! Must be numeric in the form: xxxxxx-xxxx");
                string[] dateAndLastFour = value.Split('-');
                bool isNumeric1 = int.TryParse(dateAndLastFour[0], out int n1);
                bool isNumeric2 = int.TryParse(dateAndLastFour[1], out int n2);
                bool isCorrectLength = dateAndLastFour[0].Length == 6 && dateAndLastFour[1].Length == 4;
                if (isNumeric1 && isNumeric2 && isCorrectLength)
                {
                    ssn = value;
                }
                else
                    throw new Exception("SSN is not correct! Must be numeric in the form: xxxxxx-xxxx");
            }
        }
        public string Country { get; set; }
        public string City { get; set; }
        public string Position { get; set; }
        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { if (value.Year > 0) startDate = value; else throw new Exception("Start Date must be filled."); }
        }
        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set { if (value.Year > 0) endDate = value; else throw new Exception("End Date must be filled."); }
        }
        private int workingCapacity;
        public int WorkingCapacity
        {
            get { return workingCapacity; }
            set { if (value > 0 && value < 101) workingCapacity = value; else throw new Exception("Working capacity must be filled between 1-100."); }
        }
        public List<Comment> Comments { get; set; }

        public Employee(string name, string password, string sSN, string position, string country, string city, string startDate, string endDate, int workingCapacity)
        {
            Name = name;
            Password = password;
            SSN = sSN;
            Position = position;
            Country = country;
            City = city;
            StartDate = DateTime.Parse(startDate);
            EndDate = DateTime.Parse(endDate);
            WorkingCapacity = workingCapacity;
            Comments = new List<Comment>();
        }

        public Employee()
        {
        }
    }
}
