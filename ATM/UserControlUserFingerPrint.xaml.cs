using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
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
    /// Interaction logic for UserControlUserFingerPrint.xaml
    /// </summary>
    public partial class UserControlUserFingerPrint : UserControl
    {
        LoginData data = null;
        SerialPort serial = new SerialPort();
        Customer c = new Customer();
        BackgroundWorker fingerPrint;
        bool fingerVerified = false;
        int id = 0;
        public string acn;

        public UserControlUserFingerPrint(LoginData data)
        {
            InitializeComponent();
            this.data = data;

            fingerPrint = new BackgroundWorker();
            fingerPrint.WorkerSupportsCancellation = true;
            fingerPrint.DoWork += new DoWorkEventHandler(fingerPrint_DoWork);
            fingerPrint.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fingerPrint_RunWorkerCompleted);

            foreach (string s in SerialPort.GetPortNames())
            {
                ComboBoxPort.Items.Add(s);
            }
        }

        private void fingerPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (fingerVerified == true)
            {
                Transition();
            }

            if(serial != null)
            {
                serial.Close();
            }
        }

        private void fingerPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            string rec;
            serial.Open();

            this.Dispatcher.BeginInvoke((Action)delegate () {
                RichTextBoxSerial.AppendText("Place your finger for verification\n");
            });

            this.Dispatcher.BeginInvoke((Action)delegate () {
                LabelStatus.Content = "Place finger now";
            });

            while (true)
            {
                try
                {
                    if (serial.IsOpen)
                    {
                        serial.WriteLine("2");
                    }
                    else
                    {
                        continue;
                    }

                    rec = serial.ReadLine();
                    this.Dispatcher.BeginInvoke((Action)delegate () {
                       RichTextBoxSerial.AppendText(rec);
                    });

                    if (rec.Length > 0)
                    {
                        string pattern = @"#\d+";
                        Regex rgx = new Regex(pattern);
                        MatchCollection mc = rgx.Matches(rec);
                        Match match = rgx.Match(rec);

                        if (match.Success)
                        {
                            id = Convert.ToInt32(mc[0].ToString().Replace("#", ""));

                            if (id != 0)
                            {
                                MySqlHelper helper = new MySqlHelper();
                                string connectionString = "datasource=localhost; port=3306; username=" + data.getUsername() + "; password=" + data.getPassword();
                                acn = helper.GetACN(connectionString, "db_atm", "t_customers", id);
                                fingerVerified = helper.IDConfirmed(connectionString, "db_atm", "t_customers", acn, id);

                                if (fingerVerified)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Transition()
        {
            if (fingerPrint.IsBusy)
            {
                fingerPrint.CancelAsync();
            }

            MySqlHelper helper = new MySqlHelper();
            string connectionString = "datasource=localhost; port=3306; username=" + data.getUsername() + "; password=" + data.getPassword();
            Customer customer = helper.GetCustomerWithFPID(connectionString, "db_atm", "t_customers", id);

            UserControl usc = new UserControlPerformTransaction(data, customer);
            var parent = (Grid)this.Parent;
            parent.Children.Add(usc);
            parent.Children.Remove(this);
        }

        private void ComboBoxPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                serial.PortName = ComboBoxPort.SelectedItem.ToString();
                RichTextBoxSerial.AppendText(serial.PortName + "\n");
                serial.BaudRate = Convert.ToInt32("9600");
                serial.Handshake = System.IO.Ports.Handshake.None;
                serial.Parity = Parity.None;
                serial.DataBits = 8;
                serial.StopBits = StopBits.One;
                
                fingerPrint.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
