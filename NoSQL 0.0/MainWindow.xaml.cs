using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NoSQL_0._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Database db;
        private Employee currentUser;
        private Order currenOrder;

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
            currenOrder = new Order();
            currenOrder.EmployeeId = currentUser.Id;
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
                        db.UpdateCustomerBonusPoints(customer);
                        dataGrid.ItemsSource = db.GetCustomerById(customer.Id);
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
                        currenOrder.CustomerId = customer.Id;
                        txt_addOrder_order.Text = currenOrder.ToString();
                    }
                    else if (row.Item is Item)
                    {
                        Item item = (Item)row.Item;
                        currenOrder.Items.Add(item);
                        txt_addOrder_order.Text = currenOrder.ToString();
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
                || combo_addCustomer_location.SelectedIndex == 0
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
            combo_addCustomer_location.SelectedIndex = 0;
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
                || combo_addEmployee_postition.SelectedIndex == 0
                || combo_addEmployee_location.SelectedIndex == 0
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
            combo_addEmployee_postition.SelectedIndex = 0;
            datepicker_addEmployee_startDate.SelectedDate = null;
            datepicker_addEmployee_endDate.SelectedDate = null;

            // Display new employee in datagrid
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);
            dataGrid.ItemsSource = empList;
        }

        private void Copy_cell_event(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[dataGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void btn_addOrder_searchCustomer_Click(object sender, RoutedEventArgs e)
        {
            switch (combo_addOrder_searchCustomer.SelectedIndex)
            {
                case 0:
                    dataGrid.ItemsSource = db.GetCostumerBySSN(txt_addOrder_searchCustomer.Text);
                    break;
                case 1:
                    dataGrid.ItemsSource = db.GetCostumerByCity(txt_addOrder_searchCustomer.Text);
                    break;
                case 2:
                    dataGrid.ItemsSource = db.GetCostumerByOccupation(txt_addOrder_searchCustomer.Text);
                    break;
            }
        }

        /*
         * Methods for the buttons at the bottom of the GUI
         */

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
                    dataGrid.ItemsSource = db.GetAllItems();
                    break;
                case 3:
                    dataGrid.ItemsSource = db.GetAllOrders();
                    break;
                case 4:
                    dataGrid.ItemsSource = db.GetAllComments();
                    break;
            }
        }

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

        private void txt_addOrder_order_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
