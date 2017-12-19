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
    public class Symptom
    {
        public string SymptomName;
        public int SymptomID;
        public Symptom(int symptomID, string symptomName)
        {
            SymptomID = symptomID;
            SymptomName = symptomName;
        }
        public static List<Symptom> GetSymptoms()
        {
            List<Symptom> result = new List<Symptom>();

            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL GetSymptoms()", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        result.Add(new Symptom(id, name));
                    }
                }
            }
            return result;
        }
        public static void UpdateSymptom(int id, string symptom)
        {
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL UpdateSymptom({id},'{symptom}')", connection);
                command.ExecuteNonQuery();
            }
        }

        public static string GetSymptomById(int id)
        {
            string name = "";
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetSymptomById({id})", connection);
                name = command.ExecuteScalar().ToString();
            }
            return name;
        }
    }
}
