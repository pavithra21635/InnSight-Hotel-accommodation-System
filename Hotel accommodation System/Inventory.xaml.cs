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
using System.Diagnostics;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public event Action NavigateToInventoryDetails;
        public Inventory()
        {
            InitializeComponent();
            GenerateInventoryId();
            LoadRoomNumbers();
        }

        private void GenerateInventoryId()
        {
            try
            {
                sqlcon.Open();
                string query = "SELECT TOP 1 InventoryID FROM Inventory ORDER BY InventoryID DESC";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                object result = cmd.ExecuteScalar();

                string newId = "I001"; // Default ID if no records exist

                if (result != null)
                {
                    string lastId = result.ToString();
                    int numPart = int.Parse(lastId.Substring(1)); // Extract number part
                    newId = "I" + (numPart + 1).ToString("D3"); // Increment & format
                }

                txtInventoryId.Text = newId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Inventory ID: " + ex.Message);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void LoadRoomNumbers()
        {
            try
            {
                sqlcon.Open();
                string query = "SELECT RoomID FROM Room";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                SqlDataReader reader = cmd.ExecuteReader();

                cmbRoomNumber.Items.Clear(); // Clear existing items

                while (reader.Read())
                {
                    cmbRoomNumber.Items.Add(reader["RoomID"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading room numbers: " + ex.Message);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void btn_add_item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                // Retrieve user input
                string inventoryid = txtInventoryId.Text;
                string selectedRoomid = cmbRoomNumber.SelectedItem?.ToString();
                string inventory_name = txtInventoryName.Text;
                string inventoryType = (cmbInventoryType.SelectedItem as ComboBoxItem)?.Content.ToString();

                //string quantity = cmbQuantity.SelectedItem?.ToString();


                int quantity = 0;
                if (cmbQuantity.SelectedItem is ComboBoxItem selectedItem)
                {
                    int.TryParse(selectedItem.Content.ToString(), out quantity);
                }


                // Validation
                if (string.IsNullOrEmpty(selectedRoomid) || string.IsNullOrEmpty(inventory_name) || string.IsNullOrEmpty(inventoryType) || string.IsNullOrEmpty(inventoryType))
                {
                    MessageBox.Show("Please fill all fields", "Inventory Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                else
                {

                    // SQL query to insert data
                    string query = "INSERT INTO Inventory (InventoryID, RoomID, InventoryName, InventoryType, Quantity, CreateDate, UpdatedDate) VALUES ( @InventoryID, @RoomID, @InventoryName, @InventoryType,  @Quantity, @CreateDate, @UpdatedDate)";

                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);

                    // Add parameters
                    sqlCmd.Parameters.AddWithValue("@InventoryID", inventoryid);
                    sqlCmd.Parameters.AddWithValue("@RoomID", selectedRoomid);
                    sqlCmd.Parameters.AddWithValue("@InventoryName", inventory_name);
                    sqlCmd.Parameters.AddWithValue("@InventoryType", inventoryType);
                    sqlCmd.Parameters.AddWithValue("@Quantity", quantity);
                    sqlCmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                    // Execute the query
                    int rowsAffected = sqlCmd.ExecuteNonQuery();

                    // Confirm success
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Inventory details added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
                clear();
            }
        }

        private void clear()
        {
            // Clear text fields
            txtInventoryName.Text = "";

            // Clear and reload Room Numbers
            cmbRoomNumber.Items.Clear();
            LoadRoomNumbers();  // Reload room numbers from the database

            // Reset Inventory Type
            cmbInventoryType.Items.Clear();
            cmbInventoryType.Items.Add("-- Select --");
            cmbInventoryType.Items.Add("Furniture");
            cmbInventoryType.Items.Add("Electronics");
            cmbInventoryType.Items.Add("Appliances");
            cmbInventoryType.Items.Add("Bedding");
            cmbInventoryType.Items.Add("Decor");
            cmbInventoryType.Items.Add("Bathroom Essentials");
            cmbInventoryType.Items.Add("Kitchenware");
            cmbInventoryType.Items.Add("Cleaning Supplies");
            cmbInventoryType.Items.Add("Office Equipment");
            cmbInventoryType.SelectedIndex = 0; // Set default selection

            // Reset Quantity
            cmbQuantity.Items.Clear();
            cmbQuantity.Items.Add("-- Select --");
            for (int i = 1; i <= 6; i++)
            {
                cmbQuantity.Items.Add(i.ToString());
            }
            cmbQuantity.SelectedIndex = 0; // Set default selection

            // Regenerate new Inventory ID
            GenerateInventoryId();


        }


        private void btn_update_item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                // Retrieve user input
                string inventoryId = txtUpdateInventoryId.Text;
                string roomNumber = txtUpdateRoomNumber.Text;
                string inventoryName = txtUpdateInventoryName.Text;
                string inventoryType = (cmbUpdateInventoryType.SelectedItem as ComboBoxItem)?.Content.ToString();

                int quantity = 0;
                if (cmbUpdateQuantity.SelectedItem is ComboBoxItem selectedItem)
                {
                    int.TryParse(selectedItem.Content.ToString(), out quantity);
                }

                // Validation
                if (string.IsNullOrEmpty(inventoryId) || string.IsNullOrEmpty(roomNumber) ||
                    string.IsNullOrEmpty(inventoryName) || string.IsNullOrEmpty(inventoryType))
                {
                    MessageBox.Show("Please fill all fields", "Update Inventory", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // SQL query to update data
                string query = "UPDATE Inventory SET RoomID = @RoomID, InventoryName = @InventoryName, InventoryType = @InventoryType, Quantity = @Quantity, UpdatedDate = @UpdatedDate WHERE InventoryID = @InventoryID";

                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);

                // Add parameters
                sqlCmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                sqlCmd.Parameters.AddWithValue("@RoomID", roomNumber);
                sqlCmd.Parameters.AddWithValue("@InventoryName", inventoryName);
                sqlCmd.Parameters.AddWithValue("@InventoryType", inventoryType);
                sqlCmd.Parameters.AddWithValue("@Quantity", quantity);
                sqlCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                // Execute the query
                int rowsAffected = sqlCmd.ExecuteNonQuery();

                // Confirm success
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Inventory details updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearUpdateFields(); // Clear fields after update
                }
                else
                {
                    MessageBox.Show("Failed to update inventory details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void clearUpdateFields()
        {
            txtUpdateInventoryId.Text = "";
            txtUpdateRoomNumber.Text = "";
            txtUpdateInventoryName.Text = "";
            cmbUpdateInventoryType.SelectedIndex = -1;
            cmbUpdateQuantity.SelectedIndex = -1;
        }

        private void btn_serach_inventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                string inventoryId = txtUpdateInventoryId.Text;
                if (string.IsNullOrEmpty(inventoryId))
                {
                    MessageBox.Show("Please enter an Inventory ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string query = "SELECT RoomID, InventoryName, InventoryType, Quantity FROM Inventory WHERE InventoryID = @InventoryID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUpdateRoomNumber.Text = reader["RoomID"].ToString();
                    txtUpdateInventoryName.Text = reader["InventoryName"].ToString();

                    // Select correct inventory type in ComboBox
                    foreach (ComboBoxItem item in cmbUpdateInventoryType.Items)
                    {
                        if (item.Content.ToString() == reader["InventoryType"].ToString())
                        {
                            cmbUpdateInventoryType.SelectedItem = item;
                            break;
                        }
                    }

                    // Select correct quantity in ComboBox
                    foreach (ComboBoxItem item in cmbUpdateQuantity.Items)
                    {
                        if (item.Content.ToString() == reader["Quantity"].ToString())
                        {
                            cmbUpdateQuantity.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Inventory ID not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching inventory: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void Btn_viewi_nventory_Click(object sender, RoutedEventArgs e)
        {
            NavigateToInventoryDetails?.Invoke();
        }
    }
}
