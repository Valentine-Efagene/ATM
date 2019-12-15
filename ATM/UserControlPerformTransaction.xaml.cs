using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ATM
{
    /// <summary>
    /// Interaction logic for UserControlRegisterStudents.xaml
    /// </summary>
    public partial class UserControlPerformTransaction : UserControl
    {
        LoginData data = null;
        Customer customer = null;

        public UserControlPerformTransaction(LoginData data, Customer customer)
        {
            InitializeComponent();
            this.data = data;
            this.customer = customer;
        }

        private void ButtonCashWithdraw_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = new UserControlReceipt(data, customer, "Withdrawal");
            var parent = (Grid)this.Parent;
            parent.Children.Remove(this);
            parent.Children.Add(usc);
        }

        private void ButtonCashDeposit_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = new UserControlReceipt(data, customer, "Deposit");
            var parent = (Grid)this.Parent;
            parent.Children.Remove(this);
            parent.Children.Add(usc);
        }

        private void ButtonCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = new UserControlReceipt(data, customer, "Balance check");
            var parent = (Grid)this.Parent;
            parent.Children.Remove(this);
            parent.Children.Add(usc);
        }

        private void ButtonCashTransfer_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = new UserControlReceipt(data, customer, "Cash transfer");
            var parent = (Grid)this.Parent;
            parent.Children.Remove(this);
            parent.Children.Add(usc);
        }
    }
}
