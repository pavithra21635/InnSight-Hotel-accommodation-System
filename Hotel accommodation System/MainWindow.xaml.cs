using System.Text;
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
using System.Text.RegularExpressions;


namespace Hotel_accommodation_System;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    SqlConnection sqlcon = new SqlConnection(Connection.ConnectionString);
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

        //string username = Username.Text;
        //string password = Password.Password;

        //// Validate empty fields
        //if (string.IsNullOrEmpty(username))
        //{
        //    MessageBox.Show("Username is empty.");
        //    return;
        //}
        //if (string.IsNullOrEmpty(password))
        //{
        //    MessageBox.Show("Password is empty.");
        //    return;
        //}

        //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //{
        //    MessageBox.Show("User Name and Password are empty.");
        //    return;
        //}
        //try
        //{
        //    sqlcon.Open();
        //    string query = "SELECT COUNT(1) FROM Login WHERE Username=@Username AND Password=@Password";
        //    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
        //    sqlCmd.Parameters.AddWithValue("@Username", username);
        //    sqlCmd.Parameters.AddWithValue("@Password", password);
        //    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());

        //    if (count == 1)
        //    {
        Dashboardxaml dashboard = new Dashboardxaml();
        dashboard.Show();
        this.Close();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Invalid username or password.");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show("Error: " + ex.Message);
        //}
        //finally
        //{
        //    sqlcon.Close();
        //}
    }

    private bool IsValidPassword(string password)
    {
        return password.Length > 7;
    }
}