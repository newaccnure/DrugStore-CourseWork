using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
namespace WPF_TEST
{
    public class Country
    {
        public string CountryName;
        public int CountryID;

        public Country(int countryID, string countryName)
        {
            CountryID = countryID;
            CountryName = countryName;
        }

        public static List<Country> GetCountries()
        {

            List<Country> result = new List<Country>();

            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL GetCountries()", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        result.Add(new Country(id, name));
                    }
                }

            }
            return result;
        }

        public static string GetCountryById(int id) {
            string name = "";
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetCountryById({id})", connection);
                name = command.ExecuteScalar().ToString();
            }
            return name;
        }
        public static void UpdateCountry(int id, string country) {
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL UpdateCountry({id},'{country}')", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
