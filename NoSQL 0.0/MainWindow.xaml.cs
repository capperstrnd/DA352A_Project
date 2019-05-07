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

        public MainWindow()
        {
            InitializeComponent();

            // Show login view
            gridLogin.Visibility = System.Windows.Visibility.Visible;

            // Enable copying of a row in the datagrid.
            dataGrid.SelectionMode = DataGridSelectionMode.Single;

            // Connect to database.
            db = new Database();

            // Fill datagrid with all customers
            dataGrid.ItemsSource = db.GetAllCostumers();
        }

        /*
         * Login and change view
         */
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
                // Hide the login view and show the datagrid view and the main view with the tabs.
                gridLogin.Visibility = System.Windows.Visibility.Hidden;
                gridDataGrid.Visibility = System.Windows.Visibility.Visible;
                gridMain.Visibility = System.Windows.Visibility.Visible;
            } else
            {
                System.Windows.MessageBox.Show("Password is incorrect.");
                return;
            }

            // If the user is only an employee, and not a manager, then restrict what that employee can do by disabling some tabs
            if (emp.Position.Equals("Employee"))
            {
                foreach (var tabItem in tabControl.Items)
                {
                    (tabItem as TabItem).IsEnabled = false;
                }
                tabDebugTesting.IsEnabled = true;
                tabAddCustomer.IsEnabled = true;
                tabAddOrder.IsEnabled = true;
            }

            // Keep a variable of the logged in employee.
            currentUser = emp;
            currentOrder = new Order();
            currentOrder.EmployeeId = currentUser.Id;
        }

        /*
         * Do stuff with the double clicked row in the datagrid depending on which tab you're in and which Class you clicked on.
         */
        private void gridMain_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            switch (tabControl.SelectedIndex)
            {
                // Currently debug/testing
                case 0:
                    if (row.Item is Customer)
                    {
                        Customer customer = (Customer)row.Item;
                    }
                    else if (row.Item is Item)
                    {
                        Item item = (Item)row.Item;
                    }
                    else if (row.Item is Employee)
                    {
                        Employee employee = (Employee)row.Item;
                    }
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
            }

        }

        /// <summary>
        /// This method is called when the user clicks on the 'Add Customer' button in the 'Add Customer' tab. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addCustomer_add_Click(object sender, RoutedEventArgs e)
        {
            // Input check
            if (txt_addCustomer_ssn.Text.Length < 1
                || combo_addCustomer_location.SelectedIndex == -1
                || txt_addCustomer_occupation.Text.Length < 1
                || datepicker_addCustomer.SelectedDate == null)
            {
                MessageBox.Show("All fields are not correct.");
                return;
            }

            // Create new customer
            Customer c = new Customer(
                txt_addCustomer_ssn.Text,
                combo_addCustomer_location.Text, 
                txt_addCustomer_occupation.Text, 
                0,
                datepicker_addCustomer.ToString().Split(' ')[0]);

            // Add customer to database
            db.AddCustomer(c);

            // Reset input fields
            txt_addCustomer_ssn.Text = "";
            txt_addCustomer_occupation.Text = "";
            combo_addCustomer_location.SelectedIndex = -1;
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
            // Input check
            int capacity = 0;
            if (!Int32.TryParse(txt_addEmployee_capacity.Text, out capacity) 
                || capacity > 100
                || capacity < 0
                || txt_addEmployee_name.Text.Length < 1
                || txt_addEmployee_SSN.Text.Length < 1
                || combo_addEmployee_postition.SelectedIndex == -1
                || combo_addEmployee_location.SelectedIndex == -1
                || datepicker_addEmployee_startDate.SelectedDate == null
                || datepicker_addEmployee_endDate.SelectedDate == null)
            {
                MessageBox.Show("All fields are not correct.");
                return;
            }
            
            // Create new Employee
            Employee emp = new Employee(
                txt_addEmployee_name.Text,
                "admin",
                txt_addEmployee_SSN.Text,
                combo_addEmployee_postition.Text,
                combo_addEmployee_location.Text,
                datepicker_addEmployee_startDate.ToString().Split(' ')[0],
                datepicker_addEmployee_endDate.ToString().Split(' ')[0],
                capacity);

            // Add Employee to database
            db.AddEmployee(emp);

            // Reset input fields
            txt_addEmployee_name.Text = "";
            txt_addEmployee_SSN.Text = "";
            txt_addEmployee_capacity.Text = "";
            combo_addEmployee_postition.SelectedIndex = -1;
            combo_addEmployee_location.SelectedIndex = -1;
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
        /// Method is called when the 'Search Customer' button is clicked in the 'Add Order' tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addOrder_searchCustomer_Click(object sender, RoutedEventArgs e)
        {
            switch (combo_addOrder_searchCustomer.SelectedIndex)
            {
                case 0:
                    dataGrid.ItemsSource = db.GetCustomerBySSN(txt_addOrder_searchCustomer.Text);
                    break;
                case 1:
                    dataGrid.ItemsSource = db.GetCustomerByCity(txt_addOrder_searchCustomer.Text);
                    break;
                case 2:
                    dataGrid.ItemsSource = db.GetCustomerByOccupation(txt_addOrder_searchCustomer.Text);
                    break;
            }
        }

        /// <summary>
        /// Method is called when the combobox below the datagrid is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboShowAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboShowAll.SelectedIndex)
            {
                case 0:
                    dataGrid.ItemsSource = db.GetAllCostumers();
                    break;
                case 1:
                    dataGrid.ItemsSource = db.GetAllEmployees();
                    break;
                case 2:
                    ItemStock itemStock = db.GetItemInItemStockByCity(currentUser.City);
                    dataGrid.ItemsSource = itemStock.Items;
                    break;
                case 3:
                    dataGrid.ItemsSource = db.GetAllOrders();
                    break;
                case 4:
                    dataGrid.ItemsSource = db.GetAllComments();
                    break;
            }
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
            Boolean commentAdded = db.UpdateEmployeeAddComment(id, new Comment(currentUser.Id, txt_addComment_comment.Text, "NOW"));
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
                db.UpdateItemQuantityInItemStock(currentUser.City, addedItem.Name, addedItem.Quantity);
            }

            // If buying customer is an employee.
            List<Employee> eList = db.GetEmployeesBySSN(currentOrder.CustomerId.ToString());
            if (eList.Count > 0)
            {
                // Remove 10% from total cost.
                currentOrder.TotalCost *= 0.9;
            }

            // Add todays date to the order.
            currentOrder.Date = DateTime.Today.ToString("yyyy-MM-dd");

            // Add current user city as Order city
            currentOrder.City = currentUser.City;

            // Add order to the database
            db.AddOrder(currentOrder);

            // Add a StockLog
            ItemStock itemStock = db.GetItemInItemStockByCity(currentUser.City);
            db.AddStockLog(new StockLog(DateTime.Today.ToString(), currentUser.City, itemStock));

            // Reset currentOrder
            currentOrder.Items = new List<Item>();

            // Reset view
            txt_addOrder_order.Text = "";
            dataGrid.ItemsSource = itemStock.Items;
        }

        /// <summary>
        /// Adds an item to the current order.
        /// </summary>
        /// <param name="item"></param>
        private void UpdateCurrentOrder(Item item)
        {
            // How many quantities of the item to add?
            int quantity = 0;
            Int32.TryParse(Microsoft.VisualBasic.Interaction.InputBox("How many " + item.Name + "?\n'-1' deletes all " + item.Name + " from the order.", "Add/Delete items", "1"), out quantity);

            // If quantity to add == -1 then remove that item from order.
            if (quantity == -1)
            {
                currentOrder.Items.Remove(item);
                txt_addOrder_order.Text = currentOrder.ToString();
                return;
            }

            // If item already exist in order...
            //Item stockItem = db.GetItemById(item.Id);
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

        private void btn_addOrder_resetOrder_Click(object sender, RoutedEventArgs e)
        {
            txt_addOrder_order.Text = "";
        }
    }
}
