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
        int failCount = 0;
        SerialPort serial = new SerialPort();
        Customer c = new Customer();
        BackgroundWorker fingerPrint;
        bool fingerVerified = false;
        int id = 0;
        public string acn;
        Regex rgxCommand;
        string patternCommand = @"Command"; // Command


        public UserControlUserFingerPrint(LoginData data)
        {
            InitializeComponent();
            this.data = data;

            rgxCommand = new Regex(patternCommand);
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
            else
            {
                if (serial != null)
                {
                    if (serial.IsOpen)
                    {
                        serial.Close();
                    }
                }

                UserControl usc = new UserControlWelcome(data);
                var parent = (Grid)this.Parent;
                parent.Children.Add(usc);
                parent.Children.Remove(this);
            }

            if(serial != null)
            {
                serial.Close();
            }
        }

        private void fingerPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            string pattern = @"#\d+";
            Regex rgx;
            MatchCollection mc;
            Match match;
            string rec;
            serial.Open();

            try
            {
                serial.WriteLine("2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                serial.Close();
            }

            this.Dispatcher.BeginInvoke((Action)delegate () {
                RichTextBoxSerial.AppendText("Place your finger for verification\n");
                RichTextBoxSerial.ScrollToEnd();
            });

            this.Dispatcher.BeginInvoke((Action)delegate () {
                LabelStatus.Content = "Place finger now";
            });

            while (true)
            {
                try
                {
                    try
                    {
                        serial.WriteLine("2");
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    rec = serial.ReadLine();
                    this.Dispatcher.BeginInvoke((Action)delegate () {
                        RichTextBoxSerial.AppendText(rec);
                        RichTextBoxSerial.ScrollToEnd();
                    });

                    if (rec.Length > 0)
                    {
                        pattern = @"#\d+";
                        rgx = new Regex(pattern);
                        mc = rgx.Matches(rec);
                        match = rgx.Match(rec);

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

                        pattern = @"not";
                        rgx = new Regex(pattern);
                        mc = rgx.Matches(rec);
                        match = rgx.Match(rec);

                        if (match.Success)
                        {
                            failCount++;

                            if (failCount < 3)
                            {
                                MessageBox.Show("Did not find a match.\nTry again when prompted to");
                            }
                        }

                        if (failCount == 3)
                        {
                            MessageBox.Show("ATM card temporarily disabled");

                            serial.Close();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    serial.Close();
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
