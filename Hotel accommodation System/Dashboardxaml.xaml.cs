using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for Dashboardxaml.xaml
    /// </summary>
    public partial class Dashboardxaml : Window
    {
        public Dashboardxaml()
        {
            InitializeComponent();
          ContentArea.Content = new Dashboard_Content();
        }

        private void profile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void dashboard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Reset the dashboard content
            ContentArea.Content = new Dashboard_Content();
        }

        private void Click_Room(object sender, MouseButtonEventArgs e)
        {
            ContentArea.Content = new Room();
        }

        private void employees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadEmployeePage();
        }

        private void LoadEmployeePage()
        {
            Employee employeePage = new Employee();

            // Subscribe to the event for navigating to InventoryDetails.xaml
            employeePage.NavigateToEmployeeDetails += LoadEmployeeDetailsPage;

            ContentArea.Content = employeePage;
        }
        private void LoadEmployeeDetailsPage()
        {
            EmployeeDetails detailsPage = new EmployeeDetails();

            // Subscribe to event to go back to Inventory.xaml
            detailsPage.NavigateBackToEmployee += LoadEmployeePage;

            ContentArea.Content = detailsPage;
        }

        private void customers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentArea.Content = new Customer();
        }

        private void inventories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadInventoryPage();
        }

        private void LoadInventoryPage()
        {
            Inventory inventoryPage = new Inventory();

            // Subscribe to the event for navigating to InventoryDetails.xaml
            inventoryPage.NavigateToInventoryDetails += LoadInventoryDetailsPage;

            ContentArea.Content = inventoryPage;
        }

        private void LoadInventoryDetailsPage()
        {
            InventoryDetails detailsPage = new InventoryDetails();

            // Subscribe to event to go back to Inventory.xaml
            detailsPage.NavigateBackToInventory += LoadInventoryPage;

            ContentArea.Content = detailsPage;
        }


        private void reports_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // ContentArea.Content = new Reports();
            var reportsPage = new Reports();

            // Subscribe to navigation events
            reportsPage.NavigateToCustomerReports += LoadCustomerReportsPage;
            reportsPage.NavigateToEmployeeReports += LoadEmployeeReportsPage;
            reportsPage.NavigateToInventoryReports += LoadInventoryReportsPage;
            reportsPage.NavigateToRoomDetailsReports += LoadRoomDetailsReportsPage;

            ContentArea.Content = reportsPage;
        }

        private void LoadCustomerReportsPage()
        {
            ContentArea.Content = new CustomerReports(); // Replace with your UserControl

            //var customerReportsPage = new CustomerReports();
            //customerReportsPage.NavigateBackToReports += LoadReportsPage;
            //ContentArea.Content = customerReportsPage;
        }

        private void LoadEmployeeReportsPage()
        {
            ContentArea.Content = new EmployeeReports(); // Replace with your UserControl

            //var employeeReportsPage = new EmployeeReports();
            //employeeReportsPage.NavigateBackToReports += LoadReportsPage;
            //ContentArea.Content = employeeReportsPage;
        }

        private void LoadInventoryReportsPage()
        {
            //  ContentArea.Content = new InventoryReports(); // Replace with your UserControl

            var inventoryReportsPage = new InventoryReports();
            inventoryReportsPage.NavigateBackToReports += LoadReportsPage;
            ContentArea.Content = inventoryReportsPage;
        }

        private void LoadRoomDetailsReportsPage()
        {
            //ContentArea.Content = new RoomDetailsReports(); // Replace with your UserControl
            var roomDetailsReportsPage = new RoomDetailsReports();
            roomDetailsReportsPage.NavigateBackToReports += LoadReportsPage;
            ContentArea.Content = roomDetailsReportsPage;
        }

        // Method to load Reports page again
        private void LoadReportsPage()
        {
            reports_MouseDoubleClick(null, null); // Reuse your existing method
        }
        private void RoomReservation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentArea.Content = new ReserveRoom();
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Close the current Dashboard window
            this.Close();
        }
    }
}
