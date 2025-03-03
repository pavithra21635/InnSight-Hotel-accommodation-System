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
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class Employee : UserControl
    {
        public event Action NavigateToEmployeeDetails;
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        string selectedRoomType = "0";
        string selectedBedType = "0";

        public Employee()
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

                // Query to search for the Room ID
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlcon.Open();

                string employeeId = txtempId.Text.Trim();
                string name = txtName.Text.Trim();
                string mobile = txtmobile.Text.Trim();
                string nic = txtNIC.Text.Trim();
                string address = txtAddress.Text.Trim();
                string email = txtemail.Text.Trim();
                string designation = (designationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                // Check for empty fields
                if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile) ||
                    string.IsNullOrEmpty(nic) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(designation))
                {
                    MessageBox.Show("Please fill all fields", "Employee Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate EmployeeID format
                if (!IsValidEmployeeId(employeeId))
                {
                    MessageBox.Show("Invalid Employee ID. It should be in 'EXXX' format (e.g., E123).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                string room = "SELECT COUNT(1) FROM Employee WHERE EmployeeID = @EmployeeID";
                SqlCommand Cmd = new SqlCommand(room, sqlcon);
                Cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                int exists = Convert.ToInt32(Cmd.ExecuteScalar());
                if (exists > 0)
                {
                    MessageBox.Show("Employee ID is already available. Please use a different Employee ID.", "Employee Details", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string query = "INSERT INTO Employee (EmployeeID, Name, Mobile, NIC, Address, Email, CreateDate, UpdateDate, Designation) " +
                               "VALUES (@EmployeeID, @Name, @Mobile, @NIC, @Address, @Email, @CreateDate, @UpdateDate ,@Designation )";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);

                sqlCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                sqlCmd.Parameters.AddWithValue("@Name", name);
                sqlCmd.Parameters.AddWithValue("@Mobile", mobile);
                sqlCmd.Parameters.AddWithValue("@NIC", nic);
                sqlCmd.Parameters.AddWithValue("@Address", address);
                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                sqlCmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                sqlCmd.Parameters.AddWithValue("@Designation", designation);

                int rowsAffected = sqlCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Employee details added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearFiled();
                }
                else
                {
                    MessageBox.Show("Failed to add employee details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            LoadEmployeeDetails();

        }

        private bool IsValidEmployeeId(string employeeId)
        {
            Regex regex = new Regex(@"^E\d{3}$");
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

        private void clearFiled()
        {
            txtempId.Text = "";
            txtName.Text = "";
            txtmobile.Text = "";
            txtNIC.Text = "";
            txtAddress.Text = "";
            txtemail.Text = "";
            designationComboBox.Text = "";
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string updateEmpID = txtUpdateEmpId.Text.Trim();
            string updateName = txtUpdateName.Text.Trim();
            string updateAddress = txtUpdateAddress.Text.Trim();
            string updateMobile = txtUpdatemobile.Text.Trim();
            string updateNIC = txtUpdatenic.Text.Trim();
            string updateEmail = txtUpdateemail.Text.Trim();

            // Ensure a selection is made before accessing the combo box item
            string updateDesignation = designationComboBox.SelectedItem != null
                ? (designationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
                : string.Empty;

            // Validate Employee ID
            if (string.IsNullOrWhiteSpace(updateEmpID))
            {
                MessageBox.Show("Please enter an Employee ID.", "Update Employee", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ensure the database connection is properly initialized
            if (sqlcon == null)
            {
                MessageBox.Show("Database connection is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (sqlcon.State != ConnectionState.Open)
                    sqlcon.Open();

                // Check if Employee ID exists
                string empquery = "SELECT COUNT(1) FROM Employee WHERE EmployeeID = @EmployeeID";
                using (SqlCommand checkCmd = new SqlCommand(empquery, sqlcon))
                {
                    checkCmd.Parameters.AddWithValue("@EmployeeID", updateEmpID);
                    object result = checkCmd.ExecuteScalar();
                    int exists = result != null ? Convert.ToInt32(result) : 0;

                    if (exists == 0)
                    {
                        MessageBox.Show("Employee ID does not exist.", "Update Employee", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                // Update Employee Details
                string updateQuery = @"UPDATE Employee 
                               SET Name = @Name, 
                                   Mobile = @Mobile, 
                                   NIC = @NIC, 
                                   Address = @Address, 
                                   Email = @Email, 
                                   UpdateDate = @UpdateDate, 
                                   Designation = @Designation 
                               WHERE EmployeeID = @EmployeeID";

                using (SqlCommand updateCmd = new SqlCommand(updateQuery, sqlcon))
                {
                    updateCmd.Parameters.AddWithValue("@Name", updateName);
                    updateCmd.Parameters.AddWithValue("@Mobile", updateMobile);
                    updateCmd.Parameters.AddWithValue("@NIC", updateNIC);
                    updateCmd.Parameters.AddWithValue("@Address", updateAddress);
                    updateCmd.Parameters.AddWithValue("@Email", updateEmail);
                    updateCmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                    updateCmd.Parameters.AddWithValue("@Designation", updateDesignation);
                    updateCmd.Parameters.AddWithValue("@EmployeeID", updateEmpID);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee details updated successfully.", "Update Employee", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearUpdateFields();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update employee details.", "Update Employee", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            LoadEmployeeDetails();
        }

        private void ClearUpdateFields()
        {
            txtUpdateEmpId.Clear();
            txtUpdateName.Clear();
            txtUpdateAddress.Clear();
            txtUpdatemobile.Clear();
            txtUpdatenic.Clear();
            txtUpdateemail.Clear();
            designationComboBox.SelectedIndex = -1; // Reset combo box selection
        }

        private void Btn_viewemployee_Click(object sender, RoutedEventArgs e)
        {
            NavigateToEmployeeDetails?.Invoke();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string EmployeeID = txtUpdateEmpId.Text.Trim();

            if (string.IsNullOrEmpty(EmployeeID))
            {
                MessageBox.Show("Please enter a Employee ID.", "Search Employee", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                sqlcon.Open();
                string query = "SELECT Name,Mobile, NIC,Address,Email,Designation FROM Employee WHERE EmployeeID = @EmployeeID";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUpdateName.Text = reader["Name"].ToString();
                    txtUpdateAddress.Text = reader["Address"].ToString();
                    txtUpdatemobile.Text = reader["Mobile"].ToString();
                    txtUpdatenic.Text = reader["NIC"].ToString();
                    txtUpdateemail.Text = reader["Email"].ToString();
                    //designationComboBox.SelectedValue = reader["Designation"].ToString();
                    string designation = reader["Designation"].ToString();
                    foreach (ComboBoxItem item in updatedesignationComboBox.Items)
                    {
                        if (item.Content.ToString() == designation)
                        {
                            updatedesignationComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Employee ID does not exist.", "Search Employee", MessageBoxButton.OK, MessageBoxImage.Information);
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
