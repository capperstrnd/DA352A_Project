using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoSQL_0._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Database db;
        private Employee currentUser;
        private Order currentOrder;
        private Employee employeeToUpdate;
        private Customer customerToUpdate;
        private Object searchForCollection;

        public MainWindow()
        {
            InitializeComponent();

            // Show login view
            gridLogin.Visibility = System.Windows.Visibility.Visible;

            // Enable copying of a row in the datagrid.
            dataGrid.SelectionMode = DataGridSelectionMode.Single;

            // Connect to database.
            db = new Database();

            Console.WriteLine(City.Malmö1.ToString());
        }

        /// <summary>
        /// Method is called when the 'Login' button is clicked on the login page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPass.Password;

            // Get all employees with the name typed in 'txtUser'
            var results = db.GetEmployeesByName(user);

            // If no results came back...
            if (results.Capacity == 0)
            {
                System.Windows.MessageBox.Show(user + " does not exist.");
                return;
            }

            // Pick the first employee
            Employee emp = results[0];

            // If the password typen in 'txtPass' matches the employees password...
            if (pass == emp.Password)
            {
                // Hide the login view, show the datagrid view and the main view with the tabs.
                gridLogin.Visibility = System.Windows.Visibility.Hidden;
                gridDataGrid.Visibility = System.Windows.Visibility.Visible;
                gridMain.Visibility = System.Windows.Visibility.Visible;
            } else
            {
                System.Windows.MessageBox.Show("Password is incorrect.");
                return;
            }

            // If the user is only an employee then restrict what that employee can do by disabling some tabs
            if (emp.Position == Position.Employee.ToString())
            {
                foreach (var tabItem in tabControlMain.Items)
                {
                    (tabItem as TabItem).IsEnabled = false;
                }
                tabDebugTesting.IsEnabled = true;
                tabAddCustomer.IsEnabled = true;
                tabAddOrder.IsEnabled = true;
            }

            // If the user is a location manager then restrict from producing reports.
            if (emp.Position == Position.Location_Manager.ToString())
            {
                tabAddReports.IsEnabled = false;
            }

            // Store current user as field
            currentUser = emp;
            
            // Store current order as field
            currentOrder = new Order();
            currentOrder.EmployeeId = currentUser.Id;
            
            combo_addEmployee_postition.ItemsSource = Enum.GetValues(typeof(Position));
            combo_updateEmployee_postition.ItemsSource = Enum.GetValues(typeof(Position));

            // Fill datagrid with all customers from the same country
            dataGrid.ItemsSource = CreateLocalizedItemList(db.GetItemStockByCity(currentUser.City).Items);
        }

        /// <summary>
        /// Method is called when the user double-clicks an element in the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            switch (tabControlMain.SelectedIndex)
            {
                // Currently debug/testing
                case 0:
                    break;

                // Currently Add Customer
                case 1:
                    break;

                // Currently Add Order
                case 2:
                    if (row.Item is Customer)
                    {
                        Customer customer = (Customer)row.Item;
                        currentOrder.CustomerId = customer.Id;
                        txt_addOrder_order.Text = currentOrder.ToString();
                    }
                    else if (row.Item is Item)
                    {
                        Item item = (Item)row.Item;
                        UpdateCurrentOrder(item);
                    }
                    break;

                // Currently Add Comment
                case 3:
                    if (row.Item is Employee)
                    {
                        Employee emp = (Employee)row.Item;
                        txt_addComment_id.Text = emp.Id.ToString();
                    }
                    break;
                
                // Currently Add Employee
                case 4:
                    break;
                
                // Currently Delete Employee
                case 5:
                    if (row.Item is Employee)
                    {
                        Employee employee = (Employee)row.Item;
                        var result = MessageBox.Show("Are you sure that you want to delete " + employee.Name + "?", "WARNING!", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            db.DeleteEmployee(employee);
                            dataGrid.ItemsSource = db.GetAllEmployees();
                            break;
                        }
                    }
                    break;

                // Currently Update Employee
                case 6:
                    if (row.Item is Employee)
                    {
                        Employee emp = (Employee)row.Item;
                        txt_updateEmployee_name.Text = emp.Name;
                        txt_updateEmployee_SSN.Text = emp.SSN;
                        combo_updateEmployee_postition.SelectedItem = emp.Position;
                        datepicker_updateEmployee_startDate.Text = emp.StartDate;
                        datepicker_updateEmployee_endDate.Text = emp.EndDate;
                        txt_updateEmployee_capacity.Text = emp.WorkingCapacity.ToString();
                        employeeToUpdate = emp;
                    }
                    break;

                // Currently Update Customer
                case 7:
                    if (row.Item is Customer)
                    {
                        Customer cus = (Customer)row.Item;
                        txt_updateCustomer_ssn.Text = cus.SSN;
                        txt_updateCustomer_occupation.Text = cus.Occupation;
                        combo_updateCustomer_bonusPoints.Text = cus.BonusCounter.ToString();
                        datepicker_updateCustomer.Text = cus.MembershipDate;
                        customerToUpdate = cus;
                    }
                    break;

                // Currently Add To Stock
                case 8:
                    if (row.Item is Item)
                    {
                        Item item = (Item)row.Item;
                        int quantity = 0;
                        Int32.TryParse(Microsoft.VisualBasic.Interaction.InputBox("How many " + item.Name + " to add to stock?", "Add " + item.Name + " to stock"), out quantity);
                        db.UpdateItemQuantityInItemStock(currentUser.City, item.Name, quantity, false);
                        ItemStock itemStock = db.GetItemStockByCity(currentUser.City);
                        db.AddStockLog(new StockLog(DateTime.Now.ToString(), currentUser.City, itemStock.Items));
                        dataGrid.ItemsSource = CreateLocalizedItemList(itemStock.Items);
                    }
                    break;

                // Currently Produce Reports
                case 9:
                    switch (tabControlReports.SelectedIndex)
                    {
                        // Currently Sales per date
                        case 0:
                            if (row.Item is Order && dataGrid.CurrentCell.Column.DisplayIndex == 3)
                            {
                                Order order = (Order)row.Item;
                                dataGrid.ItemsSource = CreateLocalizedItemList(order.Items);
                            }
                            break;
                    }
                    break;
            }

        }

        /// <summary>
        /// This method is called when the user clicks on the 'Add Customer' button in the 'Add Customer' tab. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addCustomer_add_Click(object sender, RoutedEventArgs e)
        {
            // Create new customer
            Customer c = null;
            try
            {
                c = new Customer(
                txt_addCustomer_ssn.Text,
                currentUser.Country,
                txt_addCustomer_occupation.Text,
                0,
                datepicker_addCustomer.ToString().Split(' ')[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create new Customer.\nReason:\n" + ex.Message);
                return;
            }

            // Add customer to database
            db.AddCustomer(c);

            // Reset input fields
            txt_addCustomer_ssn.Text = "";
            txt_addCustomer_occupation.Text = "";
            datepicker_addCustomer.SelectedDate = null;

            // Display new customer in datagrid
            List<Customer> newCustomer = new List<Customer>();
            newCustomer.Add(c);
            dataGrid.ItemsSource = newCustomer;
        }

        /// <summary>
        /// This method is called when the user clicks on the 'Add Employee' button in the 'Add Employee' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Create new Employee
            Employee emp = null;
            try
            {
                int capacity = 0;
                int.TryParse(txt_addEmployee_capacity.Text, out capacity);
                emp = new Employee(
                txt_addEmployee_name.Text,
                "admin",
                txt_addEmployee_SSN.Text,
                combo_addEmployee_postition.SelectedItem.ToString(),
                currentUser.Country,
                currentUser.City,
                datepicker_addEmployee_startDate.ToString().Split(' ')[0],
                datepicker_addEmployee_endDate.ToString().Split(' ')[0],
                capacity);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create new Employee.\nReason:\n" + ex.Message);
                return;
            }

            // Add Employee to database
            db.AddEmployee(emp);

            // Reset input fields
            txt_addEmployee_name.Text = "";
            txt_addEmployee_SSN.Text = "";
            txt_addEmployee_capacity.Text = "";
            combo_addEmployee_postition.SelectedIndex = -1;
            datepicker_addEmployee_startDate.SelectedDate = null;
            datepicker_addEmployee_endDate.SelectedDate = null;

            // Display new employee in datagrid
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);
            dataGrid.ItemsSource = empList;
        }

        /// <summary>
        /// Method is called when user ctrl+c on a cell in the datagrid.
        /// Makes it possible to copy the value of the cell to clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy_cell_event(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[dataGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        /// <summary>
        /// Method is called when the 'Logout' button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetExecutingAssembly().Location + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Method is called when the 'Add Comment' button is clicked in the 'Add Comment' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_test_add_comment_Click(object sender, RoutedEventArgs e)
        {
            ObjectId id = new ObjectId(txt_addComment_id.Text);
            Boolean commentAdded = db.UpdateEmployeeAddComment(id, new Comment(currentUser.Id, txt_addComment_comment.Text, DateTime.Now.ToString()));
            if (!commentAdded)
                MessageBox.Show("No employee with Id " + id);
            else
            {
                txt_addComment_comment.Text = "";
                txt_addComment_id.Text = "";
            }
        }

        /// <summary>
        /// Method is called when the 'Add Order' button is clicked in the 'Add Order' tab.
        /// TODO: ADD A STOCKLOG!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addOrder_add_Click(object sender, RoutedEventArgs e)
        {

            // If no items is in the order...
            if (currentOrder.Items.Count == 0)
            {
                MessageBox.Show("No items in the order.");
                return;
            }

            // Update stock quantity and update customer bonus points
            foreach (Item addedItem in currentOrder.Items)
            {
                if (addedItem.BonusItem)
                {
                    int currentBonus = db.GetCustomerById(currentOrder.CustomerId)[0].BonusCounter;
                    int bonusItems = addedItem.Quantity;
                    while (currentBonus + bonusItems >= 10)
                    {
                        currentOrder.TotalCost -= db.GetItemInItemStockByCityAndName(currentUser.City, addedItem.Name).Price;
                        bonusItems -= (11 - currentBonus);
                        currentBonus = 0;
                    }
                    currentBonus = bonusItems;
                    db.UpdateCustomerBonusPoints(db.GetCustomerById(currentOrder.CustomerId)[0], currentBonus + 1);
                }
                db.UpdateItemQuantityInItemStock(currentUser.City, addedItem.Name, addedItem.Quantity, true);
            }

            // If buying customer is an employee.
            List<Employee> eList = db.GetEmployeesBySSN(currentOrder.CustomerId.ToString());
            if (eList.Count > 0)
            {
                // Remove 10% from total cost.
                currentOrder.TotalCost *= 0.9;
            }

            // Add todays date to the order.
            currentOrder.Date = DateTime.Now;

            // Add current user city as Order city
            currentOrder.City = currentUser.City;

            // Add order to the database
            db.AddOrder(currentOrder);

            // Add a StockLog
            ItemStock itemStock = db.GetItemStockByCity(currentUser.City);
            db.AddStockLog(new StockLog(DateTime.Now.ToString(), currentUser.City, itemStock.Items));

            // Reset currentOrder
            currentOrder = new Order();
            currentOrder.EmployeeId = currentUser.Id;

            // Reset view
            txt_addOrder_order.Text = "";
            dataGrid.ItemsSource = db.GetOrderById(currentOrder.Id);
        }

        /// <summary>
        /// Adds an item to the current order.
        /// </summary>
        /// <param name="item"></param>
        private void UpdateCurrentOrder(Item item)
        {
            // How many quantities of the item to add?
            int quantity = 0;
            Int32.TryParse(Microsoft.VisualBasic.Interaction.InputBox("How many " + item.Name + "?\n'-1' deletes all " + item.Name + " from the order.", "Add/Delete items"), out quantity);

            // If quantity to add == -1 then remove that item from order.
            if (quantity == -1)
            {
                currentOrder.Items.Remove(item);
                txt_addOrder_order.Text = currentOrder.ToString();
                return;
            }

            // If item already exist in order...
            Item stockItem = db.GetItemInItemStockByCityAndName(currentUser.City, item.Name);
            if (currentOrder.Items.Contains(item))
            {
                int index = currentOrder.Items.IndexOf(item);
                currentOrder.Items[index].Quantity += quantity;
                currentOrder.Items[index].Price += stockItem.Price * quantity;
            }

            // Else, just add it to order.
            else
            {
                item.Quantity = quantity;
                item.Price = item.Price * item.Quantity;
                currentOrder.Items.Add(item);
            }

            // If stock items are out...
            if (stockItem.Quantity - item.Quantity < 0)
            {
                MessageBox.Show("Not enough " + item.Name + " in stock!");
                return;
            }

            // Reset view.
            txt_addOrder_order.Text = currentOrder.ToString();
        }

        /// <summary>
        /// Method is called when the 'Reset Order' button is clicked in the 'Add Order' tab.
        /// Clears all text in the order textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addOrder_resetOrder_Click(object sender, RoutedEventArgs e)
        {
            txt_addOrder_order.Text = "";
        }

        /// <summary>
        /// Method is called when the user clicks on the 'Update' button in the 'Update Employee' tab.
        /// Updates the information of an employee.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_updateEmployee_update_Click(object sender, RoutedEventArgs e)
        {
            // Create new Employee
            Employee emp = null;
            try
            {
                int capacity = 0;
                Int32.TryParse(txt_updateEmployee_capacity.Text, out capacity);
                emp = new Employee(
                    txt_updateEmployee_name.Text,
                    "admin",
                    txt_updateEmployee_SSN.Text,
                    combo_updateEmployee_postition.SelectedItem.ToString(),
                    currentUser.Country,
                    currentUser.City,
                    datepicker_updateEmployee_startDate.Text,
                    datepicker_updateEmployee_endDate.Text,
                    capacity);
                emp.Comments = employeeToUpdate.Comments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update Employee.\nReason:\n" + ex.Message);
                return;
            }

            // Delete current employee
            db.DeleteEmployee(employeeToUpdate);

            // Add employee
            db.AddEmployee(emp);

            // Reset input fields
            txt_updateEmployee_name.Text = "";
            txt_updateEmployee_SSN.Text = "";
            txt_updateEmployee_capacity.Text = "";
            combo_updateEmployee_postition.SelectedIndex = -1;
            datepicker_updateEmployee_startDate.SelectedDate = null;
            datepicker_updateEmployee_endDate.SelectedDate = null;

            // Show updated Employee in datagrid
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);
            dataGrid.ItemsSource = empList;
        }

        /// <summary>
        /// Method is called when the user clicks on the 'Update Customer' button is clicked in the 'Update Customer' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_updateCustomer_add_Click(object sender, RoutedEventArgs e)
        {
            // Create new customer
            
            Customer c = null;
            try
            {
                int bonusPoints = 0;
                Int32.TryParse(combo_updateCustomer_bonusPoints.Text, out bonusPoints);
                c = new Customer(
                    txt_updateCustomer_ssn.Text,
                    currentUser.Country,
                    txt_updateCustomer_occupation.Text,
                    bonusPoints,
                    datepicker_updateCustomer.ToString().Split(' ')[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update Customer.\nReason:\n" + ex.Message);
                return;
            }

            // Delete current customer
            db.DeleteCustomer(customerToUpdate);
            customerToUpdate = null;

            // Add customer to database
            db.AddCustomer(c);

            // Reset input fields
            txt_updateCustomer_ssn.Text = "";
            txt_updateCustomer_occupation.Text = "";
            datepicker_updateCustomer.SelectedDate = null;

            // Display new customer in datagrid
            List<Customer> newCustomer = new List<Customer>();
            newCustomer.Add(c);
            dataGrid.ItemsSource = newCustomer;
        }

        /// <summary>
        /// This method is called when the user clicks on the 'Search' button in the 'Produce Reports -> Sales per date' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_produceReports_salesPerDate_search_Click(object sender, RoutedEventArgs e)
        {
            // Input check
            if (datepicker_produceReports_salesPerDate_startdate.SelectedDate == null
                || datepicker_produceReports_salesPerDate_enddate.SelectedDate == null)
            {
                MessageBox.Show("Please fill in both datepickers");
                return;
            }

            DateTime start = (DateTime)datepicker_produceReports_salesPerDate_startdate.SelectedDate;
            DateTime end = (DateTime)datepicker_produceReports_salesPerDate_enddate.SelectedDate;
            dataGrid.ItemsSource = db.GetOrderBetweenDates(start, end);
        }

        /// <summary>
        /// This method is called when the user clicks on the 'Search' button in the 'Produce Reports -> Sales per item and date' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_produceReports_salesPerItemAndDate_Click(object sender, RoutedEventArgs e)
        {
            // Input check
            if (datepicker_produceReports_salesPerItemAndDate_startdate.SelectedDate == null
                || datepicker_produceReports_salesPerItemAndDate_enddate.SelectedDate == null)
            {
                MessageBox.Show("Please fill in both datepickers");
                return;
            }

            DateTime start = (DateTime)datepicker_produceReports_salesPerItemAndDate_startdate.SelectedDate;
            DateTime end = (DateTime)datepicker_produceReports_salesPerItemAndDate_enddate.SelectedDate;
            List<Order> orders = db.GetOrderBetweenDates(start, end);
            Dictionary<string, Item> soldItems = new Dictionary<string, Item>();
            foreach (Order order in orders)
            {
                foreach (Item item in order.Items)
                {
                    if (soldItems.ContainsKey(item.Name))
                    {
                        soldItems[item.Name].Quantity += item.Quantity;
                        soldItems[item.Name].Price += item.Price;
                    }
                    else
                    {
                        soldItems.Add(item.Name, item);
                    }
                }
            }
            List<Item> toShow = new List<Item>();
            foreach (Item item in soldItems.Values)
            {
                toShow.Add(item);
            }
            dataGrid.ItemsSource = toShow;
        }

        /// <summary>
        /// Method is called when the 'Collection' combobox is changed below the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combo_search_attribute.Items.Clear();
            switch (combo_search_collection.SelectedIndex)
            {
                // Customers
                case 0:
                    searchForCollection = new Customer();
                    combo_search_attribute.Items.Add("SSN");
                    combo_search_attribute.Items.Add("Occupation");
                    if (currentUser.Position == Position.Corporate_Sales_Manager.ToString())
                    {
                        combo_search_attribute.Items.Add("All");
                        txt_search_query.IsEnabled = false;
                    }
                    combo_search_attribute.SelectedIndex = 0;
                    break;

                // Employees
                case 1:
                    if (currentUser.Position != Position.Employee.ToString())
                    {
                        searchForCollection = new Employee();
                        combo_search_attribute.Items.Add("Name");
                        combo_search_attribute.Items.Add("SSN");
                        combo_search_attribute.Items.Add("All in city");
                        if (currentUser.Position == Position.Corporate_Sales_Manager.ToString())
                        {
                            combo_search_attribute.Items.Add("All");
                            txt_search_query.IsEnabled = false;
                        }
                        combo_search_attribute.SelectedIndex = 0;
                    }
                    break;
                
                // Itemstocks
                case 2:
                    searchForCollection = new Item();
                    combo_search_attribute.Items.Add("Name");
                    combo_search_attribute.Items.Add("All in city");
                    combo_search_attribute.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
        /// Method is called when the user clicks on the 'Search' button below the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = db.AllInOneSearch(searchForCollection, combo_search_attribute.Text, txt_search_query.Text, currentUser.City, currentUser.Country);
        }

        private void OnTabSelectionChanged(Object sender, SelectionChangedEventArgs args)
        {
            currentOrder = null;
            employeeToUpdate = null;
            customerToUpdate = null;
        }

        private List<Item> CreateLocalizedItemList(List<Item> oldList)
        {
            var tempList = new List<Item>(oldList);
            var ukMult = 10d;
            var usMult = 100d;

            foreach(var item in tempList)
            {
                switch (currentUser.Country)
                {
                    case "Sweden":
                        break;
                    case "England":
                        item.Price *= ukMult;
                        break;
                    case "United States":
                        item.Price *= usMult;
                        break;
                    default:
                        break;
                }
            }

            return tempList;
        }
    }
}