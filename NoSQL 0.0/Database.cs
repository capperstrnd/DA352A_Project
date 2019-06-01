using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class Database
    {

        private IMongoDatabase db;
        private IMongoCollection<Employee> colEmployee;
        private IMongoCollection<Customer> colCustomer;
        private IMongoCollection<Order> colOrder;
        private IMongoCollection<StockLog> colStockLog;
        private IMongoCollection<ItemStock> colItemStock;
        //private IMongoCollection<Item> colItem;
        private IMongoCollection<Comment> colComment;

        public Database(string[] cmdArgs)
        {
            // Connect to mongodb server running on 'localhost:27017' and get/create database 'BeaverCoffee'
            db = new MongoClient("mongodb://localhost:27017").GetDatabase("BeaverCoffee");

            // Get/create five collections in the 'BeaverCoffee' database.
            colEmployee = db.GetCollection<Employee>("Employee");
            colCustomer = db.GetCollection<Customer>("Customer");
            colOrder = db.GetCollection<Order>("Order");
            colItemStock = db.GetCollection<ItemStock>("ItemStock");
            colComment = db.GetCollection<Comment>("Comment");
            colStockLog = db.GetCollection<StockLog>("StockLog");

            foreach (string s in cmdArgs)
            {
                if(s == "buildDB") // build test database
                    addStuffToDB();
            }
        }

        /*
         *  EMPLOYEE QUERIES!
         */

        public List<Employee> GetAllEmployees()
        {
            return colEmployee.Find(x => true).ToList();
        }

        public void AddEmployee(Employee employee)
        {
            colEmployee.InsertOne(employee);
        }

        public List<Employee> GetEmployeesByCity(string employeeCity)
        {
            return colEmployee.Find(x => x.City == employeeCity.ToString()).ToList();
        }

        public List<Employee> GetEmployeesByCountry(string employeeCountry)
        {
            return colEmployee.Find(x => x.Country == employeeCountry.ToString()).ToList();
        }

        public List<Employee> GetEmployeesById(ObjectId id)
        {
            return colEmployee.Find(x => x.Id == id).ToList();
        }

        public List<Employee> GetEmployeesByName(string name)
        {
            return colEmployee.Find(x => x.Name == name).ToList();
        }

        public List<Employee> GetEmployeesBySSN(string SSN)
        {
            return colEmployee.Find(x => x.SSN == SSN).ToList();
        }

        public List<Employee> GetEmployeesByPosition(string position)
        {
            return colEmployee.Find(x => x.Position == position.ToString()).ToList();
        }

        public List<Employee> GetEmployeesByStartDate(string date)
        {
            return colEmployee.Find(x => x.StartDate == date).ToList();
        }

        public List<Employee> GetEmployeesByEndDate(string date)
        {
            return colEmployee.Find(x => x.EndDate == date).ToList();
        }

        public List<Employee> GetEmployeesByWorkingCapacity(int capacity)
        {
            return colEmployee.Find(x => x.WorkingCapacity == capacity).ToList();
        }

        public Boolean UpdateEmployeeAddComment(ObjectId employeeId, Comment comment)
        {
            if (GetEmployeesById(employeeId).Capacity == 0)
                return false;

            var filter = Builders<Employee>.Filter.Eq("_id", employeeId);
            var update = Builders<Employee>.Update.AddToSet("Comments", comment);
            var result = colEmployee.UpdateOne(filter, update);
            return true;
        }

        public void DeleteEmployee(Employee employee)
        {
            colEmployee.DeleteOneAsync(Builders<Employee>.Filter.Eq("_id", employee.Id));
        }

        /*
         * CUSTOMER QUERIES!
         */

        public List<Customer> GetAllCostumers()
        {
            return colCustomer.Find(x => true).ToList();
        }

        public void AddCustomer(Customer customer)
        {
            colCustomer.InsertOne(customer);
        }

        public List<Customer> GetCustomerById(ObjectId id)
        {
            return colCustomer.Find(x => x.Id == id).ToList();
        }

        public List<Customer> GetCustomerBySSN(string ssn)
        {
            return colCustomer.Find(x => x.SSN == ssn).ToList();
        }

        public List<Customer> GetCustomerByCountry(string country)
        {
            return colCustomer.Find(x => x.Country == country).ToList();
        }

        public List<Customer> GetCustomerByOccupation(string occupation)
        {
            return colCustomer.Find(x => x.Occupation == occupation).ToList();
        }

        public void UpdateCustomerBonusPoints(Customer customer, int newBonusPoints)
        {
            // Chose to find customer by "_id"
            var filter = Builders<Customer>.Filter.Eq("_id", customer.Id);

            // Chose which field to update and what to update it with
            var update = Builders<Customer>.Update.Set("BonusCounter", newBonusPoints);

            // Update...
            var result = colCustomer.UpdateOne(filter, update);
        }

        public void DeleteCustomer(Customer customer)
        {
            colCustomer.DeleteOneAsync(Builders<Customer>.Filter.Eq("_id", customer.Id));
        }

        /*
         * ITEMSTOCK QUERIES!
         */

        public ItemStock GetItemStockByCity(string city)
        {
            List<ItemStock> itemStock = colItemStock.Find(x => x.City == city).ToList();
            if (itemStock.Count > 0)
                return itemStock[0];
            else
                return null;
        }

        public void UpdateItemQuantityInItemStock(string city, string itemName, int quantity, bool remove)
        {
            List<ItemStock> itemStocks = colItemStock.Find(x => x.City == city).ToList();
            ItemStock itemStock = null;
            if (itemStocks.Count < 1)
                return;
            else
                itemStock = itemStocks[0];

            foreach (Item item in itemStock.Items)
            {
                if (itemName.Equals(item.Name))
                    if (remove)
                        item.Quantity -= quantity;
                    else
                        item.Quantity += quantity;
            }

            var filter = Builders<ItemStock>.Filter.Eq("_id", itemStock.Id);

            var update = Builders<ItemStock>.Update.Set("Items", itemStock.Items);

            var result = colItemStock.UpdateOne(filter, update);
        }

        public Item GetItemInItemStockByCityAndName(string city, string itemName)
        {
            List<ItemStock> itemStocks = colItemStock.Find(x => x.City == city).ToList();
            ItemStock itemStock = null;
            if (itemStocks.Count < 1)
                return null;
            else
                itemStock = itemStocks[0];

            foreach (Item item in itemStock.Items)
            {
                if (itemName.Equals(item.Name))
                    return item;
            }
            return null;
        }

        /*
         * STOCKLOG QUERIES!
         */

        public void AddStockLog(StockLog stockLog)
        {
            colStockLog.InsertOne(stockLog);
        }

        public List<StockLog> GetStockLogsBetweenDates(DateTime start, DateTime end)
        {
            return colStockLog.Find(x => x.Date >= start & x.Date <= end).ToList();
        }

        /*
         * ORDER QUERIES!
         */

        public void AddOrder(Order order)
        {
            colOrder.InsertOne(order);
        }

        public List<Order> GetAllOrders()
        {
            return colOrder.Find(x => true).ToList();
        }

        public List<Order> GetOrderById(ObjectId orderId)
        {
            return colOrder.Find(x => x.Id == orderId).ToList();
        }

        public List<Order> GetOrderBetweenDates(DateTime startDate, DateTime endDate)
        {
            return colOrder.Find(x => x.Date >= startDate & x.Date <= endDate).ToList();
        }

        public List<Order> GetOrderByCustomerId(ObjectId customerId)
        {
            return colOrder.Find(x => x.CustomerId == customerId).ToList();
        }

        /*
         * COMMENT QUERIES!
         */

        public List<Comment> GetAllComments()
        {
            return colComment.Find(x => true).ToList();
        }

        public void AddComment(Comment comment)
        {
            colComment.InsertOne(comment);
        }

        internal IEnumerable AllInOneSearch(object classType, string attribute, string query, string city, string country)
        {
            if (classType is Customer)
            {
                switch (attribute)
                {
                    case "SSN":
                        return GetCustomerBySSN(query);
                    case "Occupation":
                        return GetCustomerByOccupation(query);
                    case "All":
                        return GetAllCostumers();
                }
            }
            else if (classType is Employee)
            {
                switch (attribute)
                {
                    case "Name":
                        return GetEmployeesByName(query);
                    case "SSN":
                        return GetEmployeesBySSN(query);
                    case "All in city":
                        return GetEmployeesByCity(city);
                    case "All":
                        return GetAllEmployees();
                }
            }
            else if (classType is Item)
            {
                switch (attribute)
                {
                    case "Name":
                        List<Item> items = new List<Item>();
                        items.Add(GetItemInItemStockByCityAndName(city, query));
                        return items;
                    case "All in city":
                        return GetItemStockByCity(city).Items;
                }
                
            }
            return null;
        }

        /*
         * DEBUG STUFF
         */
        private void addStuffToDB()
        {

            /*
             * Insert employees
             */
            Employee e1 = new Employee("Mattias Sundquist", "admin", "549835-4682", Position.Location_Manager.ToString(), Country.Sweden.ToString(), City.Malmö1.ToString(), "2018-08-23", "2019-08-23", 100);
            Employee e2 = new Employee("Betty Brändström", "admin", "815462-4583", Position.Employee.ToString(), Country.Sweden.ToString(), City.Malmö2.ToString(), "2019-03-01", "2020-03-01", 100);
            Employee e3 = new Employee("Casper Strand", "admin", "690715-1234", Position.Employee.ToString(), Country.Sweden.ToString(), City.Malmö3.ToString(), "2019-05-04", "2020-05-04", 100);
            Employee e4 = new Employee("admin", "admin", "880604-1234", Position.Corporate_Sales_Manager.ToString(), Country.England.ToString(), City.London.ToString(), "2018-08-23", "2019-08-12", 100);
            colEmployee.InsertOne(e1);
            colEmployee.InsertOne(e2);
            colEmployee.InsertOne(e3);
            colEmployee.InsertOne(e4);

            /*
             * Insert costumers
             */
            Customer c1 = new Customer("884579-4568", Country.Sweden.ToString(), "Bricklayer", 4, "2017-08-20");
            Customer c2 = new Customer("915384-7538", Country.England.ToString(), "Ping pong player", 2, "2019-05-01");
            colCustomer.InsertOne(c1);
            colCustomer.InsertOne(c2);

            /*
             * Insert Items
             */
            List<Item> items = new List<Item>();
            items.Add(new Item("Esspresso Roast", 10, 1000, false));
            items.Add(new Item("Whole Bean French Roast", 10, 1000, false));
            items.Add(new Item("Qhole Bean Ligt Roast", 10, 1000, false));
            items.Add(new Item("Brewed Coffee", 10, 1000, true));
            items.Add(new Item("Espresso", 10, 1000, true));
            items.Add(new Item("Latte", 10, 1000, true));
            items.Add(new Item("Capucccino", 10, 1000, true));
            items.Add(new Item("Hot Chocolate", 10, 1000, true));
            items.Add(new Item("Skim Milk", 10, 1000, true));
            items.Add(new Item("Soy Milk", 10, 1000, true));
            items.Add(new Item("Whole Milk", 10, 1000, true));
            items.Add(new Item("2%Milk", 10, 1000, true));
            items.Add(new Item("Whipped Cream", 10, 1000, false));
            items.Add(new Item("Vanilla Syrup", 10, 1000, false));
            items.Add(new Item("Caramel Syrup", 10, 1000, false));
            items.Add(new Item("Irish Cream Syrup", 10, 1000, false));
            ItemStock itemStock = new ItemStock(City.Malmö1.ToString(), items);
            ItemStock itemStock1 = new ItemStock(City.Malmö2.ToString(), items);
            ItemStock itemStock2 = new ItemStock(City.Malmö3.ToString(), items);
            ItemStock itemStock3 = new ItemStock(City.London.ToString(), items);
            ItemStock itemStock4 = new ItemStock(City.Chicago.ToString(), items);
            ItemStock itemStock5 = new ItemStock(City.Minneapolis.ToString(), items);
            colItemStock.InsertOne(itemStock);
            colItemStock.InsertOne(itemStock1);
            colItemStock.InsertOne(itemStock2);
            colItemStock.InsertOne(itemStock3);
            colItemStock.InsertOne(itemStock4);
            colItemStock.InsertOne(itemStock5);
        }
    }
}
