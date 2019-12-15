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
    /// Interaction logic for UserControlUser.xaml
    /// </summary>
    public partial class UserControlUser : UserControl
    {
        LoginData data = null;

        public UserControlUser(LoginData data)
        {
            InitializeComponent();
            this.data = data;
            TextBoxT.Focus();
            Keyboard.Focus(TextBoxT);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UserControl usc = new UserControlUserFingerPrint(data);
                var parent = (Grid)this.Parent;
                parent.Children.Remove(this);
                parent.Children.Add(usc);
            }
        }
    }
}
