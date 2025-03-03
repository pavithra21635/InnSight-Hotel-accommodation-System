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
using System.Reflection;
using System.Data;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        string selectedRoomType = "0";
        string selectedBedType = "0";
        public Customer()
        {
            InitializeComponent();
            LoadCustomerDetails();
        }
        private void LoadCustomerDetails()
        {
            try
            {
                sqlcon.Open();

                // SQL query to fetch all room details
                string query = "SELECT CustomeRID, CusName, C_Address, CusNic, Cus_Tele,Cemail,CreateDate FROM Customer";
                SqlCommand cmd = new SqlCommand(query, sqlcon);

                // Use SqlDataAdapter to fill the DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                // Bind the data to the DataGrid
                dgCustomerDetails.ItemsSource = dataTable.DefaultView;
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

        private void Searchcustomer2_Click(object sender, RoutedEventArgs e)
        {
            string cusID = txtSearchcustomer2.Text.Trim();

            if (string.IsNullOrEmpty(cusID))
            {
                MessageBox.Show("Please enter a Customer ID to search.", "Search Customer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();

                // Query to search for the Room ID
                string query = "SELECT CustomeRID, CusName, C_Address, CusNic, Cus_Tele,Cemail,CreateDate FROM Customer WHERE CustomeRID = @CustomeRID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@CustomeRID", cusID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {

                    dgCustomerDetails.ItemsSource = dataTable.DefaultView;
                }
                else
                {

                    MessageBox.Show("Customer ID does not exist.", "Search Customer", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgCustomerDetails.ItemsSource = null;
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
            LoadCustomerDetails();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string updateCustomerID = txtUpdateCusId.Text.Trim();
            string updateName = txtUpdateName.Text.Trim();
            string updateAddress = txtUpdateAddress.Text.Trim();
            string updateMobile = txtUpdatemobile.Text.Trim();
            string updateNIC = txtUpdatenic.Text.Trim();
            string updateEmail = txtUpdateemail.Text.Trim();



            try
            {
                sqlcon.Open();
                string customerquery = "SELECT COUNT(1) FROM Customer WHERE CustomerID = @CustomerID";
                SqlCommand checkCmd = new SqlCommand(customerquery, sqlcon);
                checkCmd.Parameters.AddWithValue("@CustomerID", updateCustomerID);

                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists == 0)
                {
                    MessageBox.Show("Customer ID does not exist.", "Update Customer", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (!IsValidNIC(updateNIC))
                {
                    MessageBox.Show("Invalid NIC. Enter either 10-digit (old format) or 12-digit (new format).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate Mobile format
                if (!IsValidMobile(updateMobile))
                {
                    MessageBox.Show("Invalid Mobile Number. It should be a 10-digit number starting with '07'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate Email format
                if (!IsValidEmail(updateEmail))
                {
                    MessageBox.Show("Invalid Email format. Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string updateQuery = "UPDATE Customer SET CusName = @CusName, C_Address = @C_Address, Cus_Tele = @Cus_Tele,  CusNic = @CusNic, Cemail = @Cemail,  UpdateDate = @UpdateDate WHERE CustomerID = @CustomerID";
                SqlCommand updateCmd = new SqlCommand(updateQuery, sqlcon);
                updateCmd.Parameters.AddWithValue("@CustomerID", updateCustomerID);
                updateCmd.Parameters.AddWithValue("@CusName", updateName);
                updateCmd.Parameters.AddWithValue("@C_Address", updateAddress);
                updateCmd.Parameters.AddWithValue("@Cus_Tele", updateMobile);
                updateCmd.Parameters.AddWithValue("@CusNic", updateNIC);
                updateCmd.Parameters.AddWithValue("@Cemail", updateEmail);
                //updateCmd.Parameters.AddWithValue("@BirthOfDate", UpdatedatePickerDOB);
                updateCmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer details updated successfully.", "Update Customer", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearUpdateFields();
                }
                else
                {
                    MessageBox.Show("Failed to update customer details.", "Update customer", MessageBoxButton.OK, MessageBoxImage.Error);
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
            LoadCustomerDetails();
        }

        private void ClearUpdateFields()
        {
            txtUpdateName.Text = "";
            txtUpdateAddress.Text = "";
            txtUpdatemobile.Text = "";
            txtUpdatenic.Text = "";
            txtUpdateemail.Text = "";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string CustomerID = txtUpdateCusId.Text.Trim();

            if (string.IsNullOrEmpty(CustomerID))
            {
                MessageBox.Show("Please enter a Customer ID.", "Search Customer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();
                string query = "SELECT CusName,C_Address, Cus_Tele,CusNic,Cemail FROM Customer WHERE CustomeRID = @CustomeRID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@CustomeRID", CustomerID);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUpdateName.Text = reader["CusName"].ToString();
                    txtUpdateAddress.Text = reader["C_Address"].ToString();
                    txtUpdatemobile.Text = reader["Cus_Tele"].ToString();
                    txtUpdatenic.Text = reader["CusNic"].ToString();
                    txtUpdateemail.Text = reader["Cemail"].ToString();
                    //datePickerDOB.Text = reader["BirthOfDate"].ToString();
                }
                else
                {
                    MessageBox.Show("Customer ID does not exist.", "Search Customer", MessageBoxButton.OK, MessageBoxImage.Information);
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
            LoadCustomerDetails();
        }

        private void clearFiled()
        {
            txtAddress.Text = "";
            txtCusId.Text = "";
            txtName.Text = "";
            txtmobile.Text = "";
            txtnic.Text = "";
            txtemail.Text = "";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                string customerId = txtCusId.Text.Trim();
                string name = txtName.Text.Trim();
                string mobile = txtmobile.Text.Trim();
                string nic = txtnic.Text.Trim();
                string address = txtAddress.Text.Trim();
                string email = txtemail.Text.Trim();


                if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(nic) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Please fill all fields", "Customer Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                else
                {
                    string room = "SELECT COUNT(1) FROM Customer WHERE CustomeRID = @CustomeRID";
                    SqlCommand Cmd = new SqlCommand(room, sqlcon);
                    Cmd.Parameters.AddWithValue("@CustomeRID", customerId);

                    int exists = Convert.ToInt32(Cmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        MessageBox.Show("Customer ID is already available. Please use a different Customer ID.", "Customer Details", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!IsValidCustomerId(customerId))
                    {
                        MessageBox.Show("Invalid Customer ID. It should be in 'CXXX' format (e.g., C123).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Validate NIC format
                    if (!IsValidNIC(nic))
                    {
                        MessageBox.Show("Invalid NIC. Enter either 10-digit (old format) or 12-digit (new format).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Validate Mobile format
                    if (!IsValidMobile(mobile))
                    {
                        MessageBox.Show("Invalid Mobile Number. It should be a 10-digit number starting with '07'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Validate Email format
                    if (!IsValidEmail(email))
                    {
                        MessageBox.Show("Invalid Email format. Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string query = "INSERT INTO Customer (CustomeRID, CusName, Cus_Tele, CusNic, C_Address, Cemail,CreateDate,UpdateDate) VALUES (@CustomeRID, @CusName, @Cus_Tele, @CusNic, @C_Address,@Cemail,@CreateDate,@UpdateDate)";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);

                    sqlCmd.Parameters.AddWithValue("@CustomeRID", customerId);
                    sqlCmd.Parameters.AddWithValue("@CusName", name);
                    sqlCmd.Parameters.AddWithValue("@Cus_Tele", mobile);
                    sqlCmd.Parameters.AddWithValue("@CusNic", nic);
                    sqlCmd.Parameters.AddWithValue("@C_Address", address);
                    sqlCmd.Parameters.AddWithValue("@Cemail", email);
                    sqlCmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);

                    int rowsAffected = sqlCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Customer details added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        clearFiled();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add customer details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            }
            LoadCustomerDetails();
        }

        private bool IsValidCustomerId(string employeeId)
        {
            Regex regex = new Regex(@"^C\d{3}$");
            return regex.IsMatch(employeeId);
        }

        private bool IsValidNIC(string nic)
        {
            Regex regex = new Regex(@"^\d{9}[VvXx]$|^\d{12}$");
            return regex.IsMatch(nic);
        }

        private bool IsValidMobile(string mobile)
        {
            Regex regex = new Regex(@"^07\d{8}$");
            return regex.IsMatch(mobile);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
