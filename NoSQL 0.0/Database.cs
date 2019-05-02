using MongoDB.Bson;
using MongoDB.Driver;
using System;
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
        private IMongoCollection<Item> colItem;
        private IMongoCollection<Comment> colComment;

        public Database()
        {
            // Connect to mongodb server running on 'localhost:27017' and get/create database 'BeaverCoffee'
            db = new MongoClient("mongodb://localhost:27017").GetDatabase("BeaverCoffee");

            // Get/create five collections in the 'BeaverCoffee' database.
            colEmployee = db.GetCollection<Employee>("Employee");
            colCustomer = db.GetCollection<Customer>("Customer");
            colOrder = db.GetCollection<Order>("Order");
            colItem = db.GetCollection<Item>("Item");
            colComment = db.GetCollection<Comment>("Comment");

            // Testing and debug stuff
            //addStuffToDB();
        }

        /*
         *  GET EVERY DOCUMENT FROM EVERY COLLECTION
         */

        public List<Employee> GetAllEmployees()
        {
            return colEmployee.Find(x => true).ToList();
        }

        public List<Customer> GetAllCostumers()
        {
            Console.WriteLine("\nTest\n");
            return colCustomer.Find(x => true).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return colOrder.Find(x => true).ToList();
        }

        public List<Item> GetAllItems()
        {
            return colItem.Find(x => true).ToList();
        }

        public List<Comment> GetAllComments()
        {
            return colComment.Find(x => true).ToList();
        }

        /*
         *  ADD TO EVERY COLLECTION
         */

        public void AddCustomer(Customer customer)
        {
            colCustomer.InsertOne(customer);
        }

        public void AddEmployee(Employee employee)
        {
            colEmployee.InsertOne(employee);
        }

        public void AddItem(Item item)
        {
            colItem.InsertOne(item);
        }

        public void AddOrder(Order order)
        {
            colOrder.InsertOne(order);
        }

        public void AddComment(Comment comment)
        {
            colComment.InsertOne(comment);
        }

        /*
         * GET EMPLOYEES BY FIELDS THAT MATCHES EXACTLY 
         */

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
            return colEmployee.Find(x => x.Position == position).ToList();
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

        /*
         * GET CUSTOMERS BY FIELDS THAT MATCHES EXACTLY 
         */

        public List<Customer> GetCustomerById(ObjectId id)
        {
            return colCustomer.Find(x => x.Id == id).ToList();
        }

        public List<Customer> GetCostumerByOccupation(string occupation)
        {
            return colCustomer.Find(x => x.Occupation == occupation).ToList();
        }

        /*
         * UPDATE
         */

        public void UpdateCustomerBonusPoints(Customer customer)
        {
            // Increment bonus and save as variable
            int updatedBonus = customer.BonusCounter + 1;

            // Chose to find customer by "_id"
            var filter = Builders<Customer>.Filter.Eq("_id", customer.Id);

            // Chose which field to update and what to update it with
            var update = Builders<Customer>.Update.Set("BonusCounter", updatedBonus);

            // Update...
            var result = colCustomer.UpdateOne(filter, update);
        }

        /*
         * DEBUG STUFF
         */
        private void addStuffToDB()
        {

            /*
             * Insert two employees
             */
            Employee e1 = new Employee("Mattias Sundquist", "admin", "549835-4682", "Manager", "2018-08/23", null, 100);
            Employee e2 = new Employee("Betty Brändström", "admin", "815462-4583", "Employee", "2019-03/01", null, 100);
            Employee e3 = new Employee("admin", "admin", null, "Manager", null, null, 0);
            colEmployee.InsertOne(e1);
            colEmployee.InsertOne(e2);
            colEmployee.InsertOne(e3);

            /*
             * Insert two costumers
             */
            Customer c1 = new Customer("884579-4568", "Sweden", "Malmö", "Bricklayer", 4, "2017-08/20");
            Customer c2 = new Customer("915384-7538", "North Korea", "PlingPlong", "Ping pong player", 2, "2019-05/01");
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
            items.Add(new Item("Skim Milk,", 10, 1000, true));
            items.Add(new Item("Soy Milk,", 10, 1000, true));
            items.Add(new Item("Whole Milk", 10, 1000, true));
            items.Add(new Item("2%Milk)", 10, 1000, true));
            items.Add(new Item("Whipped Cream", 10, 1000, false));
            items.Add(new Item("Vanilla Syrup", 10, 1000, false));
            items.Add(new Item("Caramel Syrup", 10, 1000, false));
            items.Add(new Item("Irish Cream Syrup", 10, 1000, false));
            colItem.InsertMany(items);
        }

    }
}
