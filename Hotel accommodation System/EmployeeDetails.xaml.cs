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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for EmployeeDetails.xaml
    /// </summary>
    public partial class EmployeeDetails : UserControl
    {
        public event Action NavigateBackToEmployee;
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        string selectedRoomType = "0";
        string selectedBedType = "0";
        public EmployeeDetails()
        {
            InitializeComponent();
            LoadEmployeeDetails();
        }

        private void LoadEmployeeDetails()
        {
            try
            {
                sqlcon.Open();


                string query = "SELECT EmployeeID,Name, Mobile, NIC, Address, Email,Designation,CreateDate,UpdateDate FROM Employee";
                SqlCommand cmd = new SqlCommand(query, sqlcon);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dgEmployeeDetails.ItemsSource = dataTable.DefaultView;
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

        private void SearchEmployee2_Click(object sender, RoutedEventArgs e)
        {
            string empID = txtSearchEmployee.Text.Trim();

            if (string.IsNullOrEmpty(empID))
            {
                MessageBox.Show("Please enter a Employee ID to search.", "Search Employee", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();


                string query = "SELECT EmployeeID, Name, Mobile, NIC, Address,Email,Designation,CreateDate ,UpdateDate FROM Employee WHERE EmployeeID = @EmployeeID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@EmployeeID", empID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {

                    dgEmployeeDetails.ItemsSource = dataTable.DefaultView;
                }
                else
                {

                    MessageBox.Show("Employee ID does not exist.", "Search Employee", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgEmployeeDetails.ItemsSource = null;
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

        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigateBackToEmployee?.Invoke();
        }
    }
}
