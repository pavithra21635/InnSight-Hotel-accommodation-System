using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using PdfSharp.BigGustave;


namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow(string bookingId, string customerName, string mobile, string roomType, int persons, DateTime reservedDate, DateTime checkInDate, DateTime checkoutDate, string price)
        {
            InitializeComponent();

            txt_BookingID.Text = "Booking ID: " + bookingId;
            txt_BookingDate.Text = "Date / Time " + reservedDate;
            txt_CustomerName.Text = "Customer Name: " + customerName;
            txt_CustomerMobile.Text = "Mobile: " + mobile;
            txt_RoomType.Text = "Room Type: " + roomType;
            txt_NoOfPersons.Text = "No. of Persons: " + persons.ToString();
            txt_ReservedDate.Text = "Checkin Date: " + checkInDate.ToShortDateString();
            txt_CheckoutDate.Text = "Checkout Date: " + checkoutDate.ToShortDateString();
            txt_Price.Text = "Price: Rs. " + price;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
           
            // Get Desktop Path for saving
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filename = Path.Combine(desktopPath, "BookingInvoice.pdf");

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Booking Invoice";

            // Create a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Set fonts
            XFont titleFont = new XFont("Verdana", 22, XFontStyle.Bold);
            XFont headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);
            XFont redFont = new XFont("Verdana", 14, XFontStyle.Bold);

            int yPos = 50; // Vertical position
            int lineSpacing = 30;

            // 🏨 BOOKING TITLE (Top Left)
            gfx.DrawString("Booking Confirmation Invoice", titleFont, XBrushes.Black, new XPoint(50, yPos));
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


            //  yPos += 40;

            // 📌 BOOKING DETAILS
            gfx.DrawString($"Booking ID: {txt_BookingID.Text.Replace("Booking ID: ", "")}", headerFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Date / Time: {txt_BookingDate.Text.Replace("Date / Time ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Customer Name: {txt_CustomerName.Text.Replace("Customer Name: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Mobile: {txt_CustomerMobile.Text.Replace("Mobile: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Room Type: {txt_RoomType.Text.Replace("Room Type: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"No. of Persons: {txt_NoOfPersons.Text.Replace("No. of Persons: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Check-in Date: {txt_ReservedDate.Text.Replace("Checkin Date: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
            gfx.DrawString($"Checkout Date: {txt_CheckoutDate.Text.Replace("Checkout Date: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;

            // 💰 TOTAL PRICE (in Red)
            gfx.DrawString($"Total Price: {txt_Price.Text.Replace("Total : ", "")}", redFont, XBrushes.Red, new XPoint(50, yPos));
            yPos += lineSpacing + 20;

            // ➖ DRAW HORIZONTAL LINE UNDER TITLE
            gfx.DrawLine(new XPen(XColors.Black, 1.5), 50, yPos, page.Width - 50, yPos);
            yPos += 30;


            // 📝 FOOTER MESSAGE
            gfx.DrawString("Thank you for choosing River Green Hotel!", headerFont, XBrushes.DarkBlue, new XPoint(50, yPos));
            yPos += lineSpacing;
            gfx.DrawString("For any inquiries, please contact us at +94 77 123 4567", bodyFont, XBrushes.Black, new XPoint(50, yPos));

            // Save the PDF
            document.Save(filename);

            // Show success message
            MessageBox.Show($"PDF Invoice saved successfully at: {filename}", "Invoice Generated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //// Get the Desktop path
        //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //string filename = Path.Combine(desktopPath, "BookingInvoice.pdf");

        //// Create a new PDF document
        //PdfDocument document = new PdfDocument();
        //document.Info.Title = "Booking Invoice";

        //// Create an empty page
        //PdfPage page = document.AddPage();
        //XGraphics gfx = XGraphics.FromPdfPage(page);

        //// Set up fonts
        //XFont titleFont = new XFont("Verdana", 22, XFontStyle.Bold);
        //XFont headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
        //XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);

        //// Draw Invoice Title
        //gfx.DrawString("Booking Confirmation Invoice", titleFont, XBrushes.Black,
        //    new XRect(0, 30, page.Width, page.Height), XStringFormats.TopCenter);

        //// Draw Booking Details
        //int yPos = 80;
        //int lineSpacing = 30; // Space between each line

        //gfx.DrawString($"Booking ID: {txt_BookingID.Text.Replace("Booking ID: ", "")}", headerFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Date / Time: {txt_BookingDate.Text.Replace("Date / Time ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Customer Name: {txt_CustomerName.Text.Replace("Customer Name: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Mobile: {txt_CustomerMobile.Text.Replace("Mobile: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Room Type: {txt_RoomType.Text.Replace("Room Type: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"No. of Persons: {txt_NoOfPersons.Text.Replace("No. of Persons: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Check-in Date: {txt_ReservedDate.Text.Replace("Checkin Date: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Checkout Date: {txt_CheckoutDate.Text.Replace("Checkout Date: ", "")}", bodyFont, XBrushes.Black, new XPoint(50, yPos)); yPos += lineSpacing;
        //gfx.DrawString($"Total Price: {txt_Price.Text.Replace("Total : ", "")}", headerFont, XBrushes.Red, new XPoint(50, yPos)); yPos += lineSpacing;

        //// Save the document
        //document.Save(filename);

        //// Confirm success
        //MessageBox.Show($"PDF Invoice saved successfully at: {filename}", "Invoice Generated", MessageBoxButton.OK, MessageBoxImage.Information);

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the confirmation window after clicking "Confirm"
        }
    }
}
