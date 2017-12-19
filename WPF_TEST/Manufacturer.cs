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
    public class ManufacturerComparer : IEqualityComparer<Manufacturer>
    {
        public bool Equals(Manufacturer x, Manufacturer y) {
            return x.CountryID == y.CountryID;
        }
        public int GetHashCode(Manufacturer x) {
            return x.CountryID;
        }
    }
    public class Manufacturer
    {
        public int ManufacturerID;
        public string ManufacturerName;
        public int CountryID;

        public Manufacturer() { }
        public Manufacturer(int manufacturerID, string manufacturerName, int countryID)
        {
            ManufacturerID = manufacturerID;
            ManufacturerName = manufacturerName;
            CountryID = countryID;
        }

        public static List<Manufacturer> GetManufacturers() {

            List<Manufacturer> result = new List<Manufacturer>();

            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL GetFullManufacturers()", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int countryID = reader.GetInt32(2);
                        result.Add(new Manufacturer(id, name, countryID));
                    }
                }
            }
            return result;
        }

        public static Manufacturer GetManufacturerById(int id)
        {
            int countryID = 0;
            string name = "";
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetManufacturerById({id})", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        name = reader.GetString(0);
                        countryID = reader.GetInt32(1);
                    }
                }
            }
            Manufacturer manufacturer = new Manufacturer(id, name, countryID);
            return manufacturer;
        }

        public static void UpdateManufacturer(int id, string name, int countryId)
        {
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL UpdateManufacturer({id},'{name}', {countryId})", connection);
                command.ExecuteNonQuery();
            }
        }
        public static List<Tuple<int, string>> GetTopManufacturers() {
            List<Tuple<int, string>> list = new List<Tuple<int, string>>();
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetTopManufacturers()", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        list.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return list;
        }
    }
}
