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
using System.Net.Mail;
using System.Net;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for ReserveRoom.xaml
    /// </summary>
    public partial class ReserveRoom : UserControl
    {
        private string customerName = "";

        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public ReserveRoom()
        {
            InitializeComponent();
            GenerateBookingId();
            LoadRoomtypeData();
        }

        private void GenerateBookingId()
        {
            try
            {
                sqlcon.Open();
                string query = "SELECT TOP 1 ReID FROM Reservation ORDER BY ReID DESC";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                object result = cmd.ExecuteScalar();

                string newId = "B001"; // Default ID if no records exist

                if (result != null)
                {
                    string lastId = result.ToString();
                    int numPart = int.Parse(lastId.Substring(1)); // Extract number part
                    newId = "B" + (numPart + 1).ToString("D3"); // Increment & format
                }

                txt_bookingid.Text = newId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Booking ID: " + ex.Message);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void LoadRoomtypeData()
        {
            try
            {
                sqlcon.Open();
                SqlCommand roomQuery = new SqlCommand("SELECT Distinct RoomType FROM room", sqlcon);
                using (SqlDataReader reader = roomQuery.ExecuteReader())
                {
                    cmbRoomtype.Items.Clear();
                    while (reader.Read())
                    {
                        cmbRoomtype.Items.Add(reader["RoomType"].ToString());
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

        private void btn_sumbit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_cusid.Text) || string.IsNullOrEmpty(cmbroomno.Text) ||
          string.IsNullOrEmpty(cmbQuantity.Text) || dp_reservedate.SelectedDate == null || dp_checkdate.SelectedDate == null)
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // Get the values from the UI controls
            string customerId = txt_cusid.Text; // Customer ID from txt_cusid
            string roomId = cmbroomno.SelectedItem.ToString(); // Room ID from cmbroomno
                                                               // string customerName = "Pavithra Kotugala";
            string mobile = txt_mobile.Text;
            string roomType = cmbRoomtype.Text + "/" + cmbBedtype.Text;
            DateTime reservationDate = DateTime.Now;

            DateTime checkInDate = dp_reservedate.SelectedDate.Value; // Check-in date from dp_reservedate
            DateTime checkOutDate = dp_checkdate.SelectedDate.Value; // Check-out date from dp_checkdate
            string remarks = txt_remarks.Text; // Remarks from txt_remarks (assuming this exists)
            int numberOfPersons = Convert.ToInt32(((ComboBoxItem)cmbQuantity.SelectedItem).Content.ToString());

            string price = txt_price.Text;

            string bookingId = txt_bookingid.Text; // Booking ID from txt_bookingid
            string customerEmail = GetCustomerEmail(customerId, sqlcon);
            if (string.IsNullOrEmpty(customerEmail))
            {
                MessageBox.Show("Customer email not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                sqlcon.Open();

                // Insert data into the Reservation table
                SqlCommand cmd = new SqlCommand("INSERT INTO Reservation (ReID, Re_date, check_in, check_out, Remarks, No_Person, CusID, RoID) " +
                                                "VALUES (@ReID, @ReDate, @CheckInDate, @CheckOutDate, @Remarks, @NoPersons, @CusID, @RoID)", sqlcon);

                // Add parameters to avoid SQL injection
                cmd.Parameters.AddWithValue("@ReID", bookingId);
                cmd.Parameters.AddWithValue("@ReDate", reservationDate);
                cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks); // Handle null remarks
                cmd.Parameters.AddWithValue("@NoPersons", numberOfPersons);
                cmd.Parameters.AddWithValue("@CusID", customerId);
                cmd.Parameters.AddWithValue("@RoID", roomId);

                // Execute the query
                cmd.ExecuteNonQuery();

                string emailBody = $"Dear Customer,\n\n" +
                          $"Your room has been allocated successfully.\n\n" +
                          $"Booking ID: {bookingId}\n" +
                          $"Check-in Date: {checkInDate:yyyy-MM-dd}\n" +
                          $"Check-out Date: {checkOutDate:yyyy-MM-dd}\n" +
                          $"Room Type: {roomType}\n" +
                          $"Room ID: {roomId}\n" +
                          $"Total Price: {price}\n\n" +
                          $"Thank you for choosing our hotel.\n\n" +
                          $"Best Regards,\nHotel Management";

                SendEmail(customerEmail, "Room Allocation Confirmation", emailBody);
                //            MessageBox.Show("Room allocated successfully! Email sent to customer.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //   MessageBox.Show("Reservation saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Show confirmation pop-up
                ConfirmationWindow confirmation = new ConfirmationWindow(bookingId, customerName, mobile, roomType, numberOfPersons, reservationDate, checkInDate, checkOutDate, price);
                confirmation.ShowDialog(); // Open as modal window
                // Optionally, clear the form or reset fields after saving
                ClearForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database error while saving reservation: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
                clear();
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("rivergreenhotelpd@gmail.com", "tael pmms ytdx wnwu"),
                    EnableSsl = true
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("rivergreenhotelpd@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send email: " + ex.Message);
            }
        }


        private void clear()
        {
            // Clear text fields
            txt_mobile.Text = "";

            // Clear and reload Room Numbers
            cmbRoomtype.Items.Clear();
            LoadRoomtypeData();  // Reload room numbers from the database
            GenerateBookingId();

            cmbBedtype.Items.Clear();
            cmbroomno.Items.Clear();

            txt_remarks.Text = "";
        }

        private void ClearForm()
        {
            // Clear or reset the form fields if needed
            txt_cusid.Clear();
            cmbroomno.SelectedIndex = -1;
            dp_reservedate.SelectedDate = null;
            dp_checkdate.SelectedDate = null;
            txt_remarks.Text = "";
            cmbQuantity.SelectedIndex = -1;
            txt_price.Text = ""; // Clear the price text
        }

        private void txt_mobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            string mobileNumber = txt_mobile.Text;

            if (!string.IsNullOrEmpty(mobileNumber) && mobileNumber.Length == 10) // Validate phone number length
            {
                LoadCustomerId(mobileNumber);
            }
            else
            {
                txt_cusid.Text = ""; // Clear the customer ID if the mobile number is invalid
            }
        }

        private void LoadCustomerId(string mobileNumber)
        {
            try
            {
                sqlcon.Open();

                // Query to fetch CustomerID and CusName based on the mobile number
                SqlCommand cmd = new SqlCommand("SELECT CustomerID, CusName FROM Customer WHERE Cus_Tele = @MobileNumber", sqlcon);
                cmd.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                SqlDataReader reader = cmd.ExecuteReader(); // Execute the query

                if (reader.Read()) // Check if data is found
                {
                    txt_cusid.Text = reader["CustomerID"].ToString(); // Set Customer ID
                    customerName = reader["CusName"].ToString(); // Store Customer Name in variable
                }

                reader.Close(); // Close the reader
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void dp_reservedate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dp_reservedate.SelectedDate.HasValue && dp_checkdate.SelectedDate.HasValue)
            {
                if (dp_checkdate.SelectedDate.Value >= dp_reservedate.SelectedDate.Value)
                {
                    ReloadRooms();
                }
                else
                {
                    MessageBox.Show("Check-out date must be after the check-in date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void dp_checkdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dp_reservedate.SelectedDate.HasValue && dp_checkdate.SelectedDate.HasValue)
            {
                if (dp_checkdate.SelectedDate.Value >= dp_reservedate.SelectedDate.Value)
                {
                    ReloadRooms();
                }
                else
                {
                    MessageBox.Show("Check-out date must be after the check-in date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ReloadRooms()
        {
            txt_price.Text = "";
            if (cmbRoomtype.SelectedItem != null && cmbBedtype.SelectedItem != null
                && dp_reservedate.SelectedDate.HasValue && dp_checkdate.SelectedDate.HasValue)
            {
                string selectedRoomType = cmbRoomtype.SelectedItem.ToString();
                string selectedBedType = cmbBedtype.SelectedItem.ToString();
                LoadRoomid(selectedRoomType, selectedBedType);
                CalculatePrice();
            }
        }
        private void CalculatePrice()
        {
            // Check if both dates are selected
            if (dp_reservedate.SelectedDate.HasValue && dp_checkdate.SelectedDate.HasValue)
            {
                DateTime checkInDate = dp_reservedate.SelectedDate.Value;
                DateTime checkOutDate = dp_checkdate.SelectedDate.Value;

                // Ensure the check-out date is after the check-in date
                if (checkOutDate >= checkInDate)
                {
                    // Calculate the number of nights
                    int numberOfNights = (checkOutDate - checkInDate).Days;

                    // Get the room price
                    if (cmbroomno.SelectedItem != null)
                    {
                        string selectedRoomID = cmbroomno.SelectedItem.ToString();
                        decimal roomPrice = GetRoomPrice(selectedRoomID);

                        // Calculate the total price
                        decimal totalPrice = numberOfNights * roomPrice;

                        // Format the total price in LKR (Sri Lankan Rupees)

                        txt_price.Text = totalPrice.ToString(); // Format as LKR (e.g., Rs. 100.00)
                    }
                }
                else
                {
                    MessageBox.Show("Check-out date must be after the check-in date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private string GetCustomerEmail(string customerID, SqlConnection sqlcon)
        {
            string email = null;
            string query = "SELECT Cemail FROM Customer WHERE CustomerID = @CustomerID";

            // Open the connection if it is not already open
            if (sqlcon.State != System.Data.ConnectionState.Open)
            {
                sqlcon.Open();
            }

            using (SqlCommand cmd = new SqlCommand(query, sqlcon))
            {
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    email = reader["Cemail"].ToString();
                }
                reader.Close();
            }

            // Optionally, close the connection after the operation
            sqlcon.Close();

            return email;
        }

        private decimal GetRoomPrice(string roomID)
        {
            decimal price = 0;
            try
            {
                sqlcon.Open();
                SqlCommand priceQuery = new SqlCommand("SELECT Price FROM Room WHERE RoomID = @RoomID", sqlcon);
                priceQuery.Parameters.AddWithValue("@RoomID", roomID);

                object result = priceQuery.ExecuteScalar(); // Get the price as a single value

                if (result != null)
                {
                    price = Convert.ToDecimal(result); // Convert to decimal
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Database error while fetching price", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlcon.Close();
            }

            return price;
        }

        private void cmbRoomtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoomtype.SelectedItem != null)
            {
                string selectedRoomType = cmbRoomtype.SelectedItem.ToString();
                LoadBedTypes(selectedRoomType);

                // Clear price text when room type is changed
                txt_price.Text = ""; // Clear the price text
            }
        }

        private void LoadBedTypes(string roomType)
        {

            try
            {
                sqlcon.Open();
                SqlCommand bedtypeQuery = new SqlCommand("SELECT DISTINCT BedType FROM Room WHERE RoomType = @RoomType", sqlcon);
                bedtypeQuery.Parameters.AddWithValue("@RoomType", roomType);
                SqlDataReader reader = bedtypeQuery.ExecuteReader();

                cmbBedtype.Items.Clear();
                while (reader.Read())
                {
                    cmbBedtype.Items.Add(reader["BedType"].ToString());
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

        private void cmbBedtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBedtype.SelectedItem != null && cmbRoomtype.SelectedItem != null)
            {
                string selectedBedType = cmbBedtype.SelectedItem.ToString();
                string selectedRoomType = cmbRoomtype.SelectedItem.ToString();
                LoadRoomid(selectedRoomType, selectedBedType);

                // Clear price text when bed type is changed
                txt_price.Text = ""; // Clear the price text
            }
        }

        private void LoadRoomid(string roomType, string bedType)
        {

            try
            {

                // Ensure both dates are selected
                if (!dp_reservedate.SelectedDate.HasValue || !dp_checkdate.SelectedDate.HasValue)
                {
                    cmbroomno.Items.Clear();
                    return;
                }
                DateTime checkInDate = dp_reservedate.SelectedDate.Value; // Check-in date from dp_reservedate
                DateTime checkOutDate = dp_checkdate.SelectedDate.Value; // Check-out date from dp_checkdate

                sqlcon.Open();
                SqlCommand roomidQuery = new SqlCommand(@"
        SELECT DISTINCT Room.RoomID 
        FROM Room 
        LEFT JOIN Reservation ON Room.RoomID = Reservation.RoID 
        AND (
            (Reservation.check_in <= @CheckOutDate AND Reservation.check_out >= @CheckInDate)
        )
        WHERE Room.RoomType = @RoomType 
        AND Room.BedType = @BedType 
        AND Reservation.RoID IS NULL", sqlcon);

                roomidQuery.Parameters.AddWithValue("@RoomType", roomType);
                roomidQuery.Parameters.AddWithValue("@BedType", bedType);
                roomidQuery.Parameters.AddWithValue("@CheckInDate", checkInDate);
                roomidQuery.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                SqlDataReader reader = roomidQuery.ExecuteReader();

                cmbroomno.Items.Clear(); // Clear existing Room IDs

                while (reader.Read())
                {
                    cmbroomno.Items.Add(reader["RoomID"].ToString()); // Add available Room IDs to combo box
                }
                reader.Close();
                CalculatePrice();
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


        private void cmbroomno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbroomno.SelectedItem != null)
            {
                string selectedRoomID = cmbroomno.SelectedItem.ToString();
                LoadRoomPrice(selectedRoomID);

                // Recalculate price when room number is changed
                CalculatePrice();
            }
        }

        private void LoadRoomPrice(string roomID)
        {
            try
            {
                sqlcon.Open();
                SqlCommand priceQuery = new SqlCommand("SELECT Price FROM Room WHERE RoomID = @RoomID", sqlcon);
                priceQuery.Parameters.AddWithValue("@RoomID", roomID);

                object result = priceQuery.ExecuteScalar(); // Get the price as a single value

                if (result != null)
                {
                    txt_price.Text = result.ToString(); // Update txt_price with the price
                }
                else
                {
                    txt_price.Text = "N/A"; // If no price found, set default text
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Database error while fetching price", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dp_reservedate.DisplayDateStart = DateTime.Today;
            dp_checkdate.DisplayDateStart = DateTime.Today;
        }
    }
}
