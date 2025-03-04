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

namespace Hotel_accommodation_System
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        public event Action NavigateToCustomerReports;
        public event Action NavigateToEmployeeReports;
        public event Action NavigateToInventoryReports;
        public event Action NavigateToRoomDetailsReports;
        public Reports()
        {
            InitializeComponent();
        }

        private void CustomerReports_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToCustomerReports?.Invoke();
        }

        private void EmployeeReports_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToEmployeeReports?.Invoke();
        }

        private void InventoryReports_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToInventoryReports?.Invoke();
        }

        private void RoomDetailsReports_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToRoomDetailsReports?.Invoke();
        }
    }
}
