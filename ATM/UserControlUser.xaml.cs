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
using System.Windows.Threading;

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
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate ()
            {
                grid.Focus();         // Set Logical Focus
                Keyboard.Focus(grid); // Set Keyboard Focus
            }));
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                UserControl usc = new UserControlUserFingerPrint(data);
                var parent = (Grid)this.Parent;
                parent.Children.Remove(this);
                parent.Children.Add(usc);
            }
        }
    }
}
