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
        private List<Item> addedItems = new List<Item>();

        public MainWindow()
        {
            InitializeComponent();

            // Show login view
            gridLogin.Visibility = System.Windows.Visibility.Visible;

            // Enable copying of a row in the datagrid.
            dataGrid.SelectionMode = DataGridSelectionMode.Single;

            // Connect to database.
            db = new Database();
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
                        Customer c = (Customer)row.Item;
                        db.UpdateCustomerBonusPoints(c);
                        dataGrid.ItemsSource = db.GetCustomerById(c.Id);
                    }
                    else if (row.Item is Item)
                    {
                        Item c = (Item)row.Item;
                    }
                    else if (row.Item is Employee)
                    {
                        Employee c = (Employee)row.Item;
                    }
                        break;

                // Currently Add Customer
                case 1:
                    break;

                // And so on...
                case 2:
                    break;
            }

        }

        // Temporary method to show how to add customer
        private void btn_addCustomer_add_Click(object sender, RoutedEventArgs e)
        {
            Customer c = new Customer(
                txt_addCustomer_ssn.Text, 
                txt_addCustomer_country.Text, 
                txt_addCustomer_city.Text, 
                txt_addCustomer_occupation.Text, 
                0, 
                datepicker.ToString().Split(' ')[0]);

            db.AddCustomer(c);
            txt_addCustomer_ssn.Text = "";
            txt_addCustomer_country.Text = "";
            txt_addCustomer_city.Text = "";
            txt_addCustomer_occupation.Text = "";

            List<Customer> cs = new List<Customer>();
            cs.Add(c);

            dataGrid.ItemsSource = cs;
        }

        // Temporary method to show how to add employee
        private void btn_addEmployee_Click(object sender, RoutedEventArgs e)
        {
            int capacity = 0;
            if (!Int32.TryParse(txt_addEmployee_capacity.Text, out capacity) || capacity < 0 || capacity > 100)
            {
                MessageBox.Show("Working capacity incorrect");
                return;
            }

            Employee emp = new Employee(
                txt_addEmployee_name.Text,
                "admin",
                txt_addEmployee_SSN.Text,
                combo_addEmployee_postition.Text,
                datepicker_addEmployee_startDate.ToString().Split(' ')[0],
                datepicker_addEmployee_endDate.ToString().Split(' ')[0],
                capacity);

            db.AddEmployee(emp);

            txt_addEmployee_name.Text = "";
            txt_addEmployee_SSN.Text = "";
            txt_addEmployee_capacity.Text = "";
            combo_addEmployee_postition.SelectedIndex = 0;
            datepicker_addEmployee_startDate.SelectedDate = null;
            datepicker_addEmployee_endDate.SelectedDate = null;

            List<Employee> empList = new List<Employee>();
            empList.Add(emp);

            dataGrid.ItemsSource = empList;
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
    }
}
