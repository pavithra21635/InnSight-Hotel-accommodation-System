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
    /// Interaction logic for InventoryReports.xaml
    /// </summary>
    public partial class InventoryReports : UserControl
    {
        SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
        public event Action NavigateBackToReports;
        public InventoryReports()
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

                inventoryreport_datagrid.ItemsSource = dataTable.DefaultView;
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
                string filename = Path.Combine(desktopPath, "InventoryReport.pdf");

                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Inventory Report";

                // Create a page
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Set fonts
                XFont titleFont = new XFont("Verdana", 22, XFontStyle.Bold);
                XFont headerFont = new XFont("Verdana", 12, XFontStyle.Bold);
                XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);

                int yPos = 50; // Initial Y position
                int lineSpacing = 30;

                // 🏨 BOOKING TITLE (Top Left)
                gfx.DrawString("Inventory Report", titleFont, XBrushes.Black, new XPoint(50, yPos));
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

                // 📅 DATE RANGE
                gfx.DrawString($"From: {startdate_picker.SelectedDate?.ToShortDateString()}   To: {enddate_picker.SelectedDate?.ToShortDateString()}", bodyFont, XBrushes.Black, new XPoint(50, yPos));
                yPos += lineSpacing;

                // ➖ Horizontal line
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 20;

                // 📊 Table Headers
                gfx.DrawString("Inv ID", headerFont, XBrushes.Black, new XPoint(50, yPos));
                gfx.DrawString("Ro ID", headerFont, XBrushes.Black, new XPoint(120, yPos));
                gfx.DrawString("Name", headerFont, XBrushes.Black, new XPoint(190, yPos));
                gfx.DrawString("Type", headerFont, XBrushes.Black, new XPoint(280, yPos));
                gfx.DrawString("Qty", headerFont, XBrushes.Black, new XPoint(370, yPos));
                gfx.DrawString("Cre Date", headerFont, XBrushes.Black, new XPoint(420, yPos));
                gfx.DrawString("Upd Date", headerFont, XBrushes.Black, new XPoint(500, yPos));
                yPos += lineSpacing;

                // ➖ Horizontal line
                gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
                yPos += 20;

                // 📦 Add data from DataGrid
                foreach (var item in inventoryreport_datagrid.Items)
                {
                    if (item is DataRowView row)
                    {
                        gfx.DrawString(row["InventoryID"].ToString(), bodyFont, XBrushes.Black, new XPoint(50, yPos));
                        gfx.DrawString(row["RoomID"].ToString(), bodyFont, XBrushes.Black, new XPoint(120, yPos));
                        gfx.DrawString(row["InventoryName"].ToString(), bodyFont, XBrushes.Black, new XPoint(190, yPos));
                        gfx.DrawString(row["InventoryType"].ToString(), bodyFont, XBrushes.Black, new XPoint(280, yPos));
                        gfx.DrawString(row["Quantity"].ToString(), bodyFont, XBrushes.Black, new XPoint(370, yPos));
                        gfx.DrawString(Convert.ToDateTime(row["CreateDate"]).ToShortDateString(), bodyFont, XBrushes.Black, new XPoint(420, yPos));

                        if (row["UpdatedDate"] != DBNull.Value)
                        {
                            gfx.DrawString(Convert.ToDateTime(row["UpdatedDate"]).ToShortDateString(), bodyFont, XBrushes.Black, new XPoint(500, yPos));
                        }
                        else
                        {
                            gfx.DrawString("N/A", bodyFont, XBrushes.Black, new XPoint(650, yPos));
                        }

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

                // ➖ Bottom line
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

                string query = @"
            SELECT InventoryID, RoomID, InventoryName, InventoryType, Quantity, CreateDate, UpdatedDate
            FROM Inventory
            WHERE CreateDate BETWEEN @StartDate AND @EndDate
               OR (UpdatedDate IS NOT NULL AND UpdatedDate BETWEEN @StartDate AND @EndDate)";

                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    inventoryreport_datagrid.ItemsSource = null; // Clear DataGrid if no records
                    MessageBox.Show("No inventory records found within the selected date range.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    inventoryreport_datagrid.ItemsSource = dataTable.DefaultView;
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
