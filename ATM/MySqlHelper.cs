using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace ATM
{
    class MySqlHelper
    {
        
        public Customer GetCustomer(string connectionString, string databaseName, string tableName, string accountNumber)
        {
            Customer result = new Customer();
            accountNumber = accountNumber.ToUpper().Trim();
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT f_first_name, f_middle_name, f_last_name FROM " +
                databaseName + "." + tableName + " WHERE f_acn=\"" + accountNumber + "\"";

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.SetFirstName(reader["f_first_name"].ToString());
                    result.SetMiddleName(reader["f_middle_name"].ToString());
                    result.SetMiddleName(reader["f_last_name"].ToString());
                }

                connection.Close();

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public int GetIDCount(string connectionString, string databaseName, string tableName)
        {
            int result = 0;
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(f_fingerprint_id_1) FROM " + databaseName + "." + tableName;

            try
            {
                connection.Open();
                result = int.Parse(command.ExecuteScalar().ToString());

                connection.Close();

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            finally
            {
                connection.Close();
            }
        }

        public void RegisterAccount(string connectionString, string databaseName, string tableName, Customer customer)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            try
            {
                connection.Open();
                //customer.GetFingerPrints()[9] = 10;
                command.CommandText = "INSERT INTO " + databaseName + "." + tableName + " (f_acn, f_first_name, f_middle_name, f_last_name, f_fingerprint_id_1, " +
                    "f_fingerprint_id_2, f_fingerprint_id_3, f_fingerprint_id_4, f_fingerprint_id_5, f_fingerprint_id_6, f_fingerprint_id_7, " +
                    "f_fingerprint_id_8, f_fingerprint_id_9, f_fingerprint_id_10, f_email, f_phone_number) VALUES (" + 
                    "\"" + customer.GetAccountNumber() + "\"" + ", " + 
                    "\"" + customer.GetFirstName() + "\"" + ", " + 
                    "\"" + customer.GetMiddleName() + "\"" + ", " + 
                    "\"" + customer.GetLastName() + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[0] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[1] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[2] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[3] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[4] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[5] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[6] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[7] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[8] + "\"" + ", " +
                    "\"" + customer.GetFingerPrints()[9] + "\"" + ", " +
                    "\"" + customer.GetEmail() + "\"" + ", " +
                    "\"" + customer.GetPhoneNumber() + "\"" +
                    " )";

                int ret = command.ExecuteNonQuery();
                MessageBox.Show(Convert.ToString("Saving..."));

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n " + command.CommandText);
            }
            finally
            {
                connection.Close();
            }
        }

        public void RevertRegistration(string connectionString, string databaseName, string tableName,
            string matNumber)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            try
            {
                connection.Open();
                command.CommandText = "DELETE * FROM " + databaseName + "." + tableName + " WHERE f_mat_number=\"" + matNumber.ToUpper().Trim() + "\"";

                int ret = command.ExecuteNonQuery();
                MessageBox.Show(Convert.ToString("DONE"));

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n " + command.CommandText);
            }
            finally
            {
                connection.Close();
            }
        }

        public bool TestConnection(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                connection.Close();
                MessageBox.Show("Account Verified");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }

            return true;
        }

        public Customer GetCustomerWithFPID(string connectionString, string databaseName, string tableName, int id)
        {
            string firstName = null;
            string middleName = null;
            string lastName = null;
            Customer customer = new Customer();
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT f_first_name, f_middle_name, f_last_name FROM " + databaseName + "." + tableName + " WHERE f_fingerprint_id_1 = 1 or " +
                "f_fingerprint_id_2 = " + id + " or " +
                "f_fingerprint_id_3 = " + id + " or " +
                "f_fingerprint_id_4 = " + id + " or " +
                "f_fingerprint_id_5 = " + id + " or " +
                "f_fingerprint_id_6 = " + id + " or " +
                "f_fingerprint_id_7 = " + id + " or " +
                "f_fingerprint_id_8 = " + id + " or " +
                "f_fingerprint_id_9 = " + id + " or " +
                "f_fingerprint_id_10 = " + id + ";";

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    firstName = reader["f_first_name"].ToString();
                    middleName = reader["f_middle_name"].ToString();
                    lastName = reader["f_last_name"].ToString();
                }

                connection.Close();

                customer.SetFirstName(firstName);
                customer.SetMiddleName(middleName);
                customer.SetLastName(lastName);

                return customer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n " + command.CommandText);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool IDConfirmed(string connectionString, string databaseName, string tableName, string accountNumber, int id)
        {
            string acn = null;
            accountNumber = accountNumber.ToUpper().Trim();
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT f_acn FROM " +
                databaseName + "." + tableName + " WHERE f_fingerprint_id_1 = " + id;

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    acn = reader["f_acn"].ToString();
                }

                connection.Close();

                return acn == accountNumber ? true : false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n " + command.CommandText);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public string GetACN(string connectionString, string databaseName, string tableName, int id)
        {
            string acn = null;
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT f_acn FROM " +
                databaseName + "." + tableName + 
                " WHERE f_fingerprint_id_1 = " + id +
                " OR f_fingerprint_id_2 = " + id +
                " OR f_fingerprint_id_3 = " + id +
                " OR f_fingerprint_id_4 = " + id +
                " OR f_fingerprint_id_5 = " + id +
                " OR f_fingerprint_id_6 = " + id +
                " OR f_fingerprint_id_7 = " + id +
                " OR f_fingerprint_id_8 = " + id +
                " OR f_fingerprint_id_9 = " + id;

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    acn = reader["f_acn"].ToString();
                }

                connection.Close();

                return acn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n " + command.CommandText);
                return "";
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
