using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for UserControlRegister.xaml
    /// </summary>
    public partial class UserControlRegisterFingerPrints : UserControl
    {
        LoginData data = null;
        Customer customer = null;
        public SerialPort serial = new SerialPort();
        BackgroundWorker fingerPrint;
        int line = 1;
        const int N = 10;
        int id = 0;
        public string acn;
        string patternCommand = @"Command"; // Command
        string patternGetting = @"Getting"; // Getting
        string patternStored = @"Stored"; // stored
        string patternRetry = @"not"; // Re-try
        Regex rgxCommand, rgxStored, rgxRetry, rgxGetting;

        IDictionary<int, string> dict = new Dictionary<int, string>();

        int count;
        Match match;

        public UserControlRegisterFingerPrints(LoginData data, Customer customer)
        {
            InitializeComponent();
            this.data = data;
            this.customer = customer;

            rgxCommand = new Regex(patternCommand);
            rgxRetry = new Regex(patternRetry);
            rgxStored = new Regex(patternStored);
            rgxGetting = new Regex(patternGetting);
            dict.Add(1, "Right thumb");
            dict.Add(2, "Right index finger");
            dict.Add(3, "Right middle finger");
            dict.Add(4, "Right ring finger");
            dict.Add(5, "Right pinky");
            dict.Add(6, "Left thumb");
            dict.Add(7, "Left index finger");
            dict.Add(8, "Left middle finger");
            dict.Add(9, "Left ring finger");
            dict.Add(10, "Left pinky");

            fingerPrint = new BackgroundWorker();
            fingerPrint.WorkerSupportsCancellation = true;
            fingerPrint.DoWork += new DoWorkEventHandler(fingerPrint_DoWork);
            fingerPrint.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fingerPrint_RunWorkerCompleted);

            MySqlHelper helper = new MySqlHelper();
            string connectionString = "datasource=localhost; port=3306; username=" + data.getUsername() + "; password=" + data.getPassword();
            count = helper.GetIDCount(connectionString, "db_atm", "t_customers") * 10;
            id = count + 1;


            foreach (string s in SerialPort.GetPortNames())
            {
                ComboBoxPort.Items.Add(s);
            }
        }

        private void fingerPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (serial != null)
            {
                serial.Close();
            }

            Transition();
        }

        private void fingerPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            string rec = "";
            string recOld = "";
            int index = 0;

            if (serial.IsOpen)
            {
                serial.WriteLine("1");
            }
            else
            {
                MessageBox.Show("Couldn't start serial");
            }

            this.Dispatcher.BeginInvoke((Action)delegate () {
                LabelStatus.Content = "Put your " + dict[1];
            });

            while (index < N)
            {
                recOld = rec;
                rec = serial.ReadLine();

                if (rec.Trim().Equals(recOld.Trim()) || rec.Trim().Length == 0)
                {
                    continue;
                }

                this.Dispatcher.BeginInvoke((Action)delegate () {
                    RichTextBoxSerial.AppendText((line++).ToString() + ". " + rec);
                    RichTextBoxSerial.ScrollToEnd();
                });

                match = rgxGetting.Match(rec);

                if (match.Success)
                {
                    serial.WriteLine(id.ToString());
                }

                match = rgxCommand.Match(rec);

                if (match.Success)
                {
                    serial.WriteLine("1");
                }

                match = rgxStored.Match(rec);

                if (match.Success)
                {
                    customer.GetFingerPrints()[index] = id;
                    id++;
                    index++;

                    if (index < 9)
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate () {
                            LabelStatus.Content = "Put your " + dict[index + 1];
                        });
                    }
                }

                match = rgxRetry.Match(rec);

                if (match.Success)
                {
                    serial.WriteLine(id.ToString());
                    this.Dispatcher.BeginInvoke((Action)delegate () {
                        LabelStatus.Content = "Put your " + dict[index + 1] + " again";
                    });
                }
            }
        }

        private void Transition()
        {
            if (fingerPrint.IsBusy)
            {
                fingerPrint.CancelAsync();
            }

            try
            {
                MySqlHelper helper = new MySqlHelper();
                string connectionString = "datasource=localhost; port=3306; username=" + data.getUsername() + "; password=" + data.getPassword();
                helper.RegisterAccount(connectionString, "db_atm", "t_customers", customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            UserControl usc = new UserControlSaving(data, customer);
            var parent = (Grid)this.Parent;
            parent.Children.Add(usc);
            parent.Children.Remove(this);
        }

        private void Button_fingerprint_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            fingerPrint.CancelAsync();
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
                serial.Open();
                
                fingerPrint.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
