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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class UserControlEnterDetails : UserControl
    {
        LoginData data = null;
        Customer customer = new Customer();

        public UserControlEnterDetails(LoginData data)
        {
            InitializeComponent();
            this.data = data;
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxFirstName.Text == "")
            {
                MessageBox.Show("Enter your first name");
                return;
            }

            if (TextBoxMiddleName.Text == "")
            {
                MessageBox.Show("Enter your middle name");
                return;
            }

            if (TextBoxLastName.Text == "")
            {
                MessageBox.Show("Enter your last name");
                return;
            }

            if (TextBoxPhoneNumber.Text == "")
            {
                MessageBox.Show("Enter your phone number");
                return;
            }

            if (TextBoxACN.Text == "")
            {
                MessageBox.Show("Enter your account number");
                return;
            }

            customer.SetAccountNumber(TextBoxACN.Text);
            customer.SetFirstName(TextBoxFirstName.Text);
            customer.SetMiddleName(TextBoxMiddleName.Text);
            customer.SetLastName(TextBoxLastName.Text);
            customer.SetPhoneNumber(TextBoxPhoneNumber.Text);
            customer.SetEmail(TextBoxEmail.Text);

            UserControl usc = new UserControlRegisterFingerPrints(data, customer);
            var parent = (Grid)this.Parent;
            parent.Children.Remove(this);
            parent.Children.Add(usc);
        }
    }
}
