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

        }

        private void RoomReservation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
