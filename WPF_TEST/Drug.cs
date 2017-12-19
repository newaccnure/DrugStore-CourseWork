using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WPF_TEST
{
    public class Drug
    {
        public int DrugID { set; get; }
        public string WholesalePrice { set; get; }
        public string Drug_name { set; get; }
        public string Retail_price { set; get; }
        public string URL_photo { set; get; }
        public int Current_amount { set; get; }
        public string Weight_Volume { set; get; }
        public string Description { set; get; }
        public string Application { set; get; }
        public string Warning { set; get; }
        public List<Manufacturer> ManufacturersList = new List<Manufacturer>();
        public List<Symptom> SymptomsList = new List<Symptom>();
        public List<WholesalePriceClient> WPricesList = new List<WholesalePriceClient>();

        public Drug() { }
        public Drug(int drugID)
        {

            DrugID = drugID;
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetDrugById({drugID})", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        WholesalePrice = reader.GetString(1);
                        Drug_name = reader.GetString(2);
                        Retail_price = reader.GetString(3);
                        URL_photo = reader.GetString(4);
                        Current_amount = reader.GetInt32(5);
                        Weight_Volume = reader.GetString(6);
                        Description = reader.GetString(7);
                        Application = reader.GetString(8);
                        Warning = reader.GetString(9);
                    }
                }
                reader.Close();
                command.CommandText = $"CALL GetManufacturersByDrugId({drugID})";
                reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int countryID = reader.GetInt32(2);
                        ManufacturersList.Add(new Manufacturer(id, name, countryID));
                    }
                }

                reader.Close();
                command.CommandText = $"CALL GetSymptomsByDrugId({drugID})";
                reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        SymptomsList.Add(new Symptom(id, name));
                    }
                }

                reader.Close();
                command.CommandText = $"CALL GetWholesalePricesByDrugId({drugID})";
                reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        int amount = reader.GetInt32(1);
                        string price = reader.GetString(2);
                        int dID = reader.GetInt32(3);
                        WPricesList.Add(new WholesalePriceClient(id, amount, price, dID));
                    }
                }
            }
        }

        public static void UpdateDrug(Drug drug)
        {
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                    $"CALL UpdateDrug({drug.DrugID},{drug.WholesalePrice},'{drug.Drug_name}'," +
                    $"{drug.Retail_price},'{drug.URL_photo}',{drug.Current_amount}," +
                    $"'{drug.Weight_Volume}','{drug.Description}','{drug.Application}'," +
                    $"'{drug.Warning}')", connection);

                command.ExecuteNonQuery();

                command.CommandText = $"CALL DeleteAllConnectionsWithDrug({drug.DrugID})";
                command.ExecuteNonQuery();
                foreach (WholesalePriceClient wp in drug.WPricesList)
                {
                    command.CommandText = $"CALL InsertWholesalePrice({wp.Minimal_amount_of_product},{wp.Price},{drug.DrugID})";
                    command.ExecuteNonQuery();
                }

                foreach (Manufacturer m in drug.ManufacturersList)
                {
                    command.CommandText = $"CALL InsertProduct({m.ManufacturerID},{drug.DrugID})";
                    command.ExecuteNonQuery();
                }

                foreach (Symptom m in drug.SymptomsList)
                {
                    command.CommandText = $"CALL InsertTreatment({drug.DrugID},{m.SymptomID})";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddDrug(Drug drug)
        {
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                    $"CALL InsertDrug({drug.WholesalePrice},'{drug.Drug_name}'," +
                    $"{drug.Retail_price},'{drug.URL_photo}',{drug.Current_amount}," +
                    $"'{drug.Weight_Volume}','{drug.Description}','{drug.Application}'," +
                    $"'{drug.Warning}')", connection);
                int id = Convert.ToInt32(command.ExecuteScalar());

                foreach (WholesalePriceClient wp in drug.WPricesList)
                {
                    command.CommandText = $"CALL InsertWholesalePrice({wp.Minimal_amount_of_product},{wp.Price},{id})";
                    command.ExecuteNonQuery();
                }

                foreach (Manufacturer m in drug.ManufacturersList)
                {
                    command.CommandText = $"CALL InsertProduct({m.ManufacturerID},{id})";
                    command.ExecuteNonQuery();
                }

                foreach (Symptom m in drug.SymptomsList)
                {
                    command.CommandText = $"CALL InsertTreatment({id},{m.SymptomID})";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Drug> GetDrugInfoList(string filter, string search, int currentPage, int pageSize, string orderToSort)
        {

            List<Drug> drugsList = new List<Drug>();
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                    $"SELECT DISTINCT drugs.DrugID, drugs.Drug_name, drugs.Retail_price, drugs.URL_photo, drugs.Current_amount " +
                    $"FROM drugs, manufacturers, products, symptoms, treatments, countries" +
                    $" WHERE(drugs.DrugID = products.DrugID " +
                    $"AND drugs.DrugID = treatments.DrugID " +
                    $"AND manufacturers.ManufacturerID = products.ManufacturerID " +
                    $"AND symptoms.SymptomID = treatments.SymptomID " +
                    $"AND countries.CountryID = manufacturers.CountryID) " +
                    $"{filter} AND {search}" +
                    $"ORDER BY {orderToSort} " +
                    $"LIMIT {pageSize} " +
                    $"OFFSET {currentPage * pageSize};", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Drug drug = new Drug
                        {
                            DrugID = reader.GetInt32(0),
                            Drug_name = reader.GetString(1),
                            Retail_price = reader.GetString(2),
                            URL_photo = reader.GetString(3),
                            Current_amount = reader.GetInt32(4)
                        };
                        drugsList.Add(drug);
                    }
                }
            }

            return drugsList;
        }

        public static string AddPurchase(List<Tuple<Drug, int, string>> purchases, string login)
        {
            string chequeID = "";
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("", connection);
                foreach (Tuple<Drug, int, string> purchase in purchases)
                {
                    command.CommandText = $"CALL InsertPurchase(" +
                        $"{purchase.Item1.DrugID}, {purchase.Item2}, " +
                        $"{purchase.Item3.Replace(',', '.')}, " +
                        $"'{login}')";
                    chequeID += command.ExecuteScalar().ToString() + '_';
                    //command.ExecuteNonQuery();
                }
            }
            return chequeID;
        }

        public static List<Drug> GetAllDrugs()
        {
            List<Drug> drugs = new List<Drug>();
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL GetDrugs()", connection);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        Drug drug = new Drug(id);
                        drugs.Add(drug);
                    }
                }
            }
            return drugs;
        }

        public static List<Tuple<int, string>> GetSalesPerMonthByID(int id) {
            List<Tuple<int, string>> res = new List<Tuple<int, string>>();
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT MIN(MONTH(Date_of_purchase)), MONTH(now()) FROm historyofsales where drugid = {id}; ", connection);
                MySqlDataReader reader = command.ExecuteReader();
                int beginMonth = 0;
                int curMonth = 0;
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        beginMonth = reader.GetInt32(0);
                        curMonth = reader.GetInt32(1);
                    }
                }
                for (int i = beginMonth; i <= curMonth; i++)
                {
                    reader.Close();
                    command.CommandText = $"CALL GetMonthSalesForDrug({id},{i})";
                    reader = command.ExecuteReader();
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            res.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }

            return res;
        }

        public static List<Drug> GetRatingSortedDrugList(string filter,
            string search, int currentPage, int pageSize)
        {
            List<Drug> drugsList = new List<Drug>();
            filter = filter.Substring(filter.IndexOf(' ')+1);
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                   $"SELECT DISTINCT drugs.DrugID, drugs.Drug_name, drugs.Retail_price, drugs.URL_photo, drugs.Current_amount " +
                   $"FROM((((((drugs " +
                   $"JOIN products ON drugs.DrugID = products.DrugID) " +
                   $"JOIN treatments ON drugs.DrugID = treatments.DrugID) " +
                   $"JOIN manufacturers ON manufacturers.ManufacturerID = products.ManufacturerID) " +
                   $"JOIN symptoms ON symptoms.SymptomID = treatments.SymptomID) " +
                   $"JOIN countries ON countries.CountryID = manufacturers.CountryID) " +
                   $"LEFT JOIN historyofsales ON drugs.DrugID = historyofsales.DrugID) " +
                   $"WHERE { filter } AND { search} " +
                   $"GROUP BY drugs.DrugID " +
                   $"ORDER BY COUNT(DISTINCT historyofsales.PurchaseID) DESC " +
                   $"LIMIT {pageSize} " +
                   $"OFFSET {currentPage * pageSize};", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Drug drug = new Drug
                        {
                            DrugID = reader.GetInt32(0),
                            Drug_name = reader.GetString(1),
                            Retail_price = reader.GetString(2),
                            URL_photo = reader.GetString(3),
                            Current_amount = reader.GetInt32(4)
                        };
                        drugsList.Add(drug);
                    }
                }

            }
            return drugsList;
        }
        public static List<Tuple<int, string>> GetTopDrugs()
        {
            List<Tuple<int, string>> list = new List<Tuple<int, string>>();
            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"CALL GetDrugsPurchasesStats()", connection);
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
