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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for InventoryDetails.xaml
    /// </summary>
    public partial class InventoryDetails : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public event Action NavigateBackToInventory;
        public InventoryDetails()
        {
            InitializeComponent();
            LoadInventoryDetails();
        }

        private void LoadInventoryDetails()
        {
            try
            {
                sqlcon.Open();

                string query = "SELECT InventoryID,RoomID,InventoryName,InventoryType,Quantity,CreateDate,UpdatedDate FROM Inventory";
                SqlCommand cmd = new SqlCommand(query, sqlcon);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dgInventoryDetails.ItemsSource = dataTable.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }
        }
        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigateBackToInventory?.Invoke();
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            string inventoryID = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(inventoryID))
            {
                MessageBox.Show("Please enter a Inventory ID to search.", "Search Inventory", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();

                string query = "SELECT InventoryID,RoomID,InventoryName,InventoryType,Quantity,CreateDate,UpdatedDate FROM Inventory WHERE InventoryID = @InventoryID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@InventoryID", inventoryID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {

                    dgInventoryDetails.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Inventory ID does not exist.", "Search Inventory", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgInventoryDetails.ItemsSource = null; // Clear DataGrid if no results
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }

        }
    }
}
