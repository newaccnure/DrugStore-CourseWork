using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WPF_TEST
{
    public class Logic
    {
        public static string connectionString = "server = localhost; user id = root; persistsecurityinfo=True;database=drugstore;password=2699nes24";

        public Logic() {}
        public static void ShowTable(DataGrid dataGrid, string sqlQ) {
            using (MySqlConnection connection = new MySqlConnection(connectionString)) {
                MySqlCommand command = new MySqlCommand(sqlQ, connection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.DataContext = dataTable.DefaultView;
                dataGrid.Columns[0].Visibility = Visibility.Collapsed;
            }
        }

        public static bool CheckLogin(string login) {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL CheckLoginQuery('{login}')", connection);
                int number = Convert.ToInt32(command.ExecuteScalar());

                if (number == 0) {
                    return true;
                }
                return false;
            }
        }
        public static bool CheckUser(string login, string password) {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string sqlQ = $"CALL CheckUserQuery('{login}','{password}')";
                MySqlCommand command = new MySqlCommand(sqlQ, con);

                int number = Convert.ToInt32(command.ExecuteScalar());
                
                if (number == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public static int NumberOfDrugs()
        {
            int number = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString)) {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL NumberOfDrugs()", connection);
                number = Convert.ToInt32(command.ExecuteScalar());
            }
            return number;
        }

        public static List<string> GetListID(UIElementCollection uiElementCollection, List<KeyValuePair<string,int>> dictionary) {
            List<string> result = new List<string>();

            int i = 0;
            foreach (CheckBox checkBox in uiElementCollection)
            {
                if ((bool)checkBox.IsChecked)
                {
                    result.Add(dictionary[i].Value.ToString());
                }
                i++;
            }

            return result;
        }

        public static void InsertInformation(string sqlQ) {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sqlQ, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void DeleteInformation(string sqlQ)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sqlQ, connection);
                command.ExecuteNonQuery();
            }
        }

        public static string IncomeRequest(string time) {
            return $"SELECT drugs.DrugID, drugs.Drug_name as 'Название', " +
                $"COUNT(historyofsales.PurchaseID) as 'Количество покупок', " +
                $"SUM(historyofsales.Price * historyofsales.Amount_of_purchased_product) - drugs.Wholesale_Price * SUM(historyofsales.Amount_of_purchased_product) as 'Прибыль', " +
                $"SUM(historyofsales.Amount_of_purchased_product * historyofsales.Price) as 'Доход', " +
                $"drugs.Wholesale_Price* SUM(historyofsales.Amount_of_purchased_product) as 'Затраты', " +
                $"drugs.Wholesale_Price as 'Цена закупки', " +
                $"SUM(historyofsales.Amount_of_purchased_product) as 'Количество проданного товара' " +
                $"FROM(drugs JOIN historyofsales ON drugs.DrugID = historyofsales.DrugID) " +
                time +
                $"GROUP BY drugs.DrugID " +
                $"ORDER BY drugs.Drug_name;";
        }
    }
}
