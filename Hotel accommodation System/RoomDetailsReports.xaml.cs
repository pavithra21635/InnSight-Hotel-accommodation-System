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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using PdfSharp.BigGustave;

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for RoomDetailsReports.xaml
    /// </summary>
    public partial class RoomDetailsReports : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public event Action NavigateBackToReports;
        public RoomDetailsReports()
        {
            InitializeComponent();
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
                Roomreport_datagrid.ItemsSource = dataTable.DefaultView;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateBackToReports?.Invoke();
        }

        private void download_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get Desktop Path for saving
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filename = Path.Combine(desktopPath, "RoomDetailsReport.pdf");

                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Room Details Report";

                // Create a page
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Set fonts
                XFont titleFont = new XFont("Verdana", 22, XFontStyle.Bold);
                XFont headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
                XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);
                XFont redFont = new XFont("Verdana", 14, XFontStyle.Bold);

                int yPos = 50; // Initial Y position
                int lineSpacing = 30;

                // 🏨 BOOKING TITLE (Top Left)
                gfx.DrawString("Room Details Report", titleFont, XBrushes.Black, new XPoint(50, yPos));
                yPos += 10;

                // ➖ DRAW HORIZONTAL LINE UNDER TITLE
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 20;




                // 🏨 DRAW HOTEL LOGO (Optional)
                //  string logoPath = "C:\\Users\\pavit\\source\\repos\\Hotel accomodation\\Hotel_accomodation_system\\nibm222comp_E_Hotel_accomodation_system\\Black & Blue Minimalist Modern Initial Font Logo 12.png"; // Replace with actual path
                // string logoPath = "\\MainwindiowBlack & Blue Minimalist Modern.png"; // Replace with actual path

                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MainwindiowBlack & Blue Minimalist Modern.png");




                if (File.Exists(logoPath))
                {
                    XImage logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 30, yPos, 100, 100);
                    yPos += 110;
                }

                // 🏨 HOTEL INFORMATION
                gfx.DrawString("River Green Hotel", titleFont, XBrushes.DarkBlue, new XPoint(150, 105));
                gfx.DrawString("123 Beach Road, Colombo, Sri Lanka", bodyFont, XBrushes.Black, new XPoint(150, 125));
                gfx.DrawString("Phone: +94 77 123 4567 | Email: contact@luxurystay.com", bodyFont, XBrushes.Black, new XPoint(150, 145));
                gfx.DrawString("Website: www.luxurystay.com", bodyFont, XBrushes.Black, new XPoint(150, 165));

                // ➖ DRAW HORIZONTAL LINE UNDER TITLE
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 40;

                

                gfx.DrawString($"From: {startdate_picker.SelectedDate?.ToShortDateString()}   To: {enddate_picker.SelectedDate?.ToShortDateString()}", bodyFont, XBrushes.Black, new XPoint(50, yPos));
                yPos += lineSpacing;

                // ➖ Horizontal line
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 20;

                // 📊 Table Headers
                gfx.DrawString("Room ID", headerFont, XBrushes.Black, new XPoint(50, yPos));
                gfx.DrawString("Room Type", headerFont, XBrushes.Black, new XPoint(150, yPos));
                gfx.DrawString("Bed Type", headerFont, XBrushes.Black, new XPoint(280, yPos));
                gfx.DrawString("Price", headerFont, XBrushes.Black, new XPoint(380, yPos));
                gfx.DrawString("Created Date", headerFont, XBrushes.Black, new XPoint(480, yPos));
                yPos += lineSpacing;

                // ➖ Horizontal line
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 20;

                // 📄 Add data from DataGrid
                foreach (var item in Roomreport_datagrid.Items)
                {
                    if (item is DataRowView row)
                    {
                        gfx.DrawString(row["RoomID"].ToString(), bodyFont, XBrushes.Black, new XPoint(50, yPos));
                        gfx.DrawString(row["RoomType"].ToString(), bodyFont, XBrushes.Black, new XPoint(150, yPos));
                        gfx.DrawString(row["BedType"].ToString(), bodyFont, XBrushes.Black, new XPoint(280, yPos));
                        gfx.DrawString($"Rs. {row["Price"]}", bodyFont, XBrushes.Black, new XPoint(380, yPos));
                        gfx.DrawString(Convert.ToDateTime(row["CreateDate"]).ToShortDateString(), bodyFont, XBrushes.Black, new XPoint(510, yPos));
                        yPos += lineSpacing;

                        // Ensure page doesn't overflow
                        if (yPos > page.Height - 50)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            yPos = 50;
                        }
                    }
                }

                // ➖ Horizontal line at bottom
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 30;

                // 🏁 Footer
                gfx.DrawString("Generated on: " + DateTime.Now.ToString("f"), bodyFont, XBrushes.Gray, new XPoint(50, yPos));
                gfx.DrawString("Thank you for choosing River Green Hotel!", headerFont, XBrushes.DarkBlue, new XPoint(50, yPos + lineSpacing));

                // Save PDF
                document.Save(filename);

                // Show confirmation
                MessageBox.Show($"PDF Report saved successfully at: {filename}", "Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void startdate_picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startdate_picker.SelectedDate.HasValue && enddate_picker.SelectedDate.HasValue)
            {
                if (enddate_picker.SelectedDate.Value >= startdate_picker.SelectedDate.Value)
                {
                    LoadRoomDetailsByDateRange();
                }
                else
                {
                    MessageBox.Show("To date must be after the From date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void enddate_picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startdate_picker.SelectedDate.HasValue && enddate_picker.SelectedDate.HasValue)
            {
                if (enddate_picker.SelectedDate.Value >= startdate_picker.SelectedDate.Value)
                {
                    LoadRoomDetailsByDateRange();
                }
                else
                {
                    MessageBox.Show("To date must be after the From date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }


        private void LoadRoomDetailsByDateRange()
        {
            try
            {
                if (!startdate_picker.SelectedDate.HasValue || !enddate_picker.SelectedDate.HasValue)
                {
                    return;
                }

                DateTime startDate = startdate_picker.SelectedDate.Value;
                DateTime endDate = enddate_picker.SelectedDate.Value;

                sqlcon.Open();

                string query = @"SELECT RoomID, RoomType, BedType, Price, CreateDate 
                                FROM Room 
                                WHERE CreateDate BETWEEN @StartDate AND @EndDate";

                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                //  Roomreport_datagrid.ItemsSource = dataTable.DefaultView;

                if (dataTable.Rows.Count == 0)
                {
                    Roomreport_datagrid.ItemsSource = null; // Clear the DataGrid
                    MessageBox.Show("No records between selected dates.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Roomreport_datagrid.ItemsSource = dataTable.DefaultView;
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
