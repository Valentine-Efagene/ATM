using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for UserControlSaving.xaml
    /// </summary>
    public partial class UserControlSaving : UserControl
    {
        LoginData data = null;
        Customer customer = null;

        public UserControlSaving(LoginData data, Customer customer)
        {
            InitializeComponent();
            this.data = data;
            this.customer = customer;
            //MessageBox.Show("Saving...");
            LabelStatus.Content = "Done";
        }
    }
}
