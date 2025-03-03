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
    /// Interaction logic for Room.xaml
    /// </summary>
    public partial class Room : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        string selectedRoomType = "0";
        string selectedBedType = "0";

        public Room()
        {
            InitializeComponent();
            LoadData();
            LoadUpdateData();
            LoadRoomDetails();
        }

        private void LoadRoomDetails()
        {
            try
            {
                sqlcon.Open();

                // SQL query to fetch all room details
                string query = "SELECT RoomID, RoomType, BedType, Price, CreateDate FROM Room";
                SqlCommand cmd = new SqlCommand(query, sqlcon);

                // Use SqlDataAdapter to fill the DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                // Bind the data to the DataGrid
                dgRoomDetails.ItemsSource = dataTable.DefaultView;
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

        private void LoadData()
        {
            try
            {
                sqlcon.Open();
                SqlCommand roomQuery = new SqlCommand("SELECT Distinct RoomType FROM room", sqlcon);
                using (SqlDataReader reader = roomQuery.ExecuteReader())
                {
                    cmbRoomType.Items.Clear();
                    while (reader.Read())
                    {
                        cmbRoomType.Items.Add(reader["RoomType"].ToString());
                    }
                }

                SqlCommand cusQuery = new SqlCommand("SELECT Distinct BedType FROM room", sqlcon);
                using (SqlDataReader reader = cusQuery.ExecuteReader())
                {
                    cmbBedType.Items.Clear();
                    while (reader.Read())
                    {
                        cmbBedType.Items.Add(reader["BedType"].ToString());
                    }
                }



            }
            catch (SqlException)
            {
                MessageBox.Show("Database error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void LoadUpdateData()
        {
            try
            {
                sqlcon.Open();

                // Populate RoomType ComboBox for Update Section
                SqlCommand roomQuery = new SqlCommand("SELECT DISTINCT RoomType FROM room", sqlcon);
                using (SqlDataReader reader = roomQuery.ExecuteReader())
                {
                    cmbUpdateRoomType.Items.Clear(); // Use the ComboBox for the Update section
                    while (reader.Read())
                    {
                        cmbUpdateRoomType.Items.Add(reader["RoomType"].ToString());
                    }
                }

                // Populate BedType ComboBox for Update Section
                SqlCommand bedQuery = new SqlCommand("SELECT DISTINCT BedType FROM room", sqlcon);
                using (SqlDataReader reader = bedQuery.ExecuteReader())
                {
                    cmbUpdateBedType.Items.Clear(); // Use the ComboBox for the Update section
                    while (reader.Read())
                    {
                        cmbUpdateBedType.Items.Add(reader["BedType"].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                // Retrieve user input
                string roomID = txtRoomId.Text.Trim();
                string selectedRoomType = cmbRoomType.SelectedItem?.ToString();
                string selectedBedType = cmbBedType.SelectedItem?.ToString();
                string totalPrice = txtPrice.Text.Trim();

                // Validation
                if (string.IsNullOrEmpty(roomID) || string.IsNullOrEmpty(selectedRoomType) || string.IsNullOrEmpty(selectedBedType) || string.IsNullOrEmpty(totalPrice))
                {
                    MessageBox.Show("Please fill all fields", "Room Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (!IsValidRoomId(roomID))
                {
                    MessageBox.Show("Room ID format is incorrect. Please enter in the format R001, R002, etc.", "Room Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                else if (!decimal.TryParse(totalPrice, out decimal price))
                {
                    MessageBox.Show("Invalid price. Please enter a valid numeric value.", "Room Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                else
                {

                    // Check if RoomID already exists
                    string checkQuery = "SELECT COUNT(1) FROM Room WHERE RoomID = @RoomID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, sqlcon);
                    checkCmd.Parameters.AddWithValue("@RoomID", roomID);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        MessageBox.Show("Room ID is already available. Please use a different Room ID.", "Room Details", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // SQL query to insert data
                    string query = "INSERT INTO Room (RoomID, RoomType, BedType, Price, CreateDate, UpdatedDate) VALUES (@RoomID, @RoomType, @BedType, @Price, @CreateDate, @UpdatedDate)";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);

                    // Add parameters
                    sqlCmd.Parameters.AddWithValue("@RoomID", roomID);
                    sqlCmd.Parameters.AddWithValue("@RoomType", selectedRoomType);
                    sqlCmd.Parameters.AddWithValue("@BedType", selectedBedType);
                    sqlCmd.Parameters.AddWithValue("@Price", price);
                    sqlCmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                    // Execute the query
                    int rowsAffected = sqlCmd.ExecuteNonQuery();

                    // Confirm success
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Room details added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        clear();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add room details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
                LoadRoomDetails();
            }
        }

        private bool IsValidRoomId(string roomId)
        {
            Regex regex = new Regex(@"^R\d{3}$");
            return regex.IsMatch(roomId);
        }

        private void clear()
        {
            cmbRoomType.Items.Clear();
            cmbBedType.Items.Clear();

            // Reset Inventory Type
            cmbRoomType.Items.Clear();
            cmbRoomType.Items.Add("-- Select --");
            cmbRoomType.Items.Add("AC-Single");
            cmbRoomType.Items.Add("NONAC-Single");
            cmbRoomType.Items.Add("AC-Double");
            cmbRoomType.Items.Add("NONAC-Double");
            cmbRoomType.Items.Add("AC-Suite");
            cmbRoomType.SelectedIndex = 0; // Set default selection

            cmbBedType.Items.Clear();
            cmbBedType.Items.Add("-- Select --");
            cmbBedType.Items.Add("Queen");
            cmbBedType.Items.Add("King");
            cmbBedType.SelectedIndex = 0; // Set default selection



            txtRoomId.Text = "";
            txtPrice.Text = "";
        }
        private void selectRoomType(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoomType.SelectedItem != null)
            {
                this.selectedRoomType = cmbRoomType.SelectedItem.ToString();

            }
        }

        private void selectBedType(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBedType.SelectedItem != null)
            {
                this.selectedBedType = cmbBedType.SelectedItem.ToString();

            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string roomID = txtUpdateRoomId.Text.Trim();

            if (string.IsNullOrEmpty(roomID))
            {
                MessageBox.Show("Please enter a Room ID.", "Search Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();

                // Query to check if the Room ID exists
                string query = "SELECT RoomType, BedType, Price FROM Room WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@RoomID", roomID);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Room exists, populate the fields
                    cmbUpdateRoomType.SelectedItem = reader["RoomType"].ToString();
                    cmbUpdateBedType.SelectedItem = reader["BedType"].ToString();
                    txtUpdatePrice.Text = reader["Price"].ToString();
                }
                else
                {
                    // Room ID does not exist
                    MessageBox.Show("Room ID does not exist.", "Search Room", MessageBoxButton.OK, MessageBoxImage.Information);
                    //ClearUpdateFields();
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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string roomID = txtUpdateRoomId.Text.Trim();
            string selectedRoomType = cmbUpdateRoomType.SelectedItem?.ToString();
            string selectedBedType = cmbUpdateBedType.SelectedItem?.ToString();
            string updatedPrice = txtUpdatePrice.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(roomID) || string.IsNullOrEmpty(selectedRoomType) || string.IsNullOrEmpty(selectedBedType) || string.IsNullOrEmpty(updatedPrice))
            {
                MessageBox.Show("Please fill all fields.", "Update Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!decimal.TryParse(updatedPrice, out decimal price))
            {
                MessageBox.Show("Invalid price format. Please enter a valid numeric value.", "Update Room", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                sqlcon.Open();

                // Check if Room ID exists in the database
                string checkQuery = "SELECT COUNT(1) FROM Room WHERE RoomID = @RoomID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, sqlcon);
                checkCmd.Parameters.AddWithValue("@RoomID", roomID);

                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists == 0)
                {
                    MessageBox.Show("Room ID does not exist.", "Update Room", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Update the room details
                string updateQuery = "UPDATE Room SET RoomType = @RoomType, BedType = @BedType, Price = @Price, UpdatedDate = @UpdatedDate WHERE RoomID = @RoomID";
                SqlCommand updateCmd = new SqlCommand(updateQuery, sqlcon);
                updateCmd.Parameters.AddWithValue("@RoomType", selectedRoomType);
                updateCmd.Parameters.AddWithValue("@BedType", selectedBedType);
                updateCmd.Parameters.AddWithValue("@Price", updatedPrice);
                updateCmd.Parameters.AddWithValue("@RoomID", roomID);
                updateCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Room details updated successfully.", "Update Room", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearUpdateFields();
                }
                else
                {
                    MessageBox.Show("Failed to update room details.", "Update Room", MessageBoxButton.OK, MessageBoxImage.Error);
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
                LoadRoomDetails();
            }
        }

        private void ClearUpdateFields()
        {
            txtUpdateRoomId.Text = "";
            cmbUpdateRoomType.SelectedIndex = -1;
            cmbUpdateBedType.SelectedIndex = -1;
            txtUpdatePrice.Text = "";
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e)
        {
            string roomID = txtSearch2.Text.Trim();

            if (string.IsNullOrEmpty(roomID))
            {
                MessageBox.Show("Please enter a Room ID to search.", "Search Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();

                // Query to search for the Room ID
                string query = "SELECT RoomID, RoomType, BedType, Price, CreateDate FROM Room WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@RoomID", roomID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    // Room ID exists, load data into DataGrid
                    dgRoomDetails.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    // Room ID does not exist
                    MessageBox.Show("Room ID does not exist.", "Search Room", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgRoomDetails.ItemsSource = null; // Clear DataGrid if no results
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
