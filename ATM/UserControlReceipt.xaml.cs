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

namespace ATM
{
    /// <summary>
    /// Interaction logic for UserControlReceipt.xaml
    /// </summary>
    public partial class UserControlReceipt : UserControl
    {
        LoginData data = null;
        Customer customer = null;
        string description = "";

        public UserControlReceipt(LoginData data, Customer customer, string description)
        {
            InitializeComponent();
            this.data = data;
            this.customer = customer;
            this.description = description;
            labelName.Content = customer.GetFirstName() + " " + customer.GetMiddleName() + " " + customer.GetLastName();
            labelDescription.Content = description;
        }
    }
}
