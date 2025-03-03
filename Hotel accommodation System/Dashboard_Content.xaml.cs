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
    /// Interaction logic for Dashboard_Content.xaml
    /// </summary>
    public partial class Dashboard_Content : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public Dashboard_Content()
        {
            InitializeComponent();
            LoadReservationDetails();
        }

        private void LoadReservationDetails()
        {
            try
            {
                sqlcon.Open();

                string query = "SELECT ReID, Re_date, check_in, check_out, RoID,Remarks, No_Person, CusID FROM Reservation";
                SqlCommand cmd = new SqlCommand(query, sqlcon);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dgBookingDetails.ItemsSource = dataTable.DefaultView;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string reservationID = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(reservationID))
            {
                MessageBox.Show("Please enter a Reservation ID to search.", "Search Reservation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();

                string query = "SELECT ReID, Re_date, check_in, check_out, RoID, Remarks, No_Person, CusID FROM Reservation WHERE ReID = @ReID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@ReID", reservationID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    dgBookingDetails.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Reservation ID does not exist.", "Search Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgBookingDetails.ItemsSource = null; // Clear DataGrid if no results
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
