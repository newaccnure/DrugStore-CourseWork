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
using System.Data.Sql;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using Word = Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string currentTable;
        public Dictionary<string, string> queriesDictionary;
        public List<DrugAnalysis> listOfAnalys = new List<DrugAnalysis>();
        public MainWindow()
        {
            InitializeComponent();

            List<Tuple<int, string, int, int, int, int, int>> drugsPurchase = new List<Tuple<int, string, int, int, int, int, int>>();

            using (MySqlConnection connection = new MySqlConnection(Logic.connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL AutomatizationStatistics()", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int cur_amount = reader.GetInt32(2);
                        int beginMonth = reader.GetInt32(3);
                        int middleMonth = reader.GetInt32(4);
                        int endMonth = reader.GetInt32(5);
                        double beginCoef = Convert.ToDouble(middleMonth) / beginMonth;
                        double endCoef = Convert.ToDouble(endMonth) / middleMonth;
                        int beginAngle = middleMonth - beginMonth;
                        int endAngle = endMonth - middleMonth;
                        int amountToHave = 0;
                        if (beginAngle >= 0 && endAngle >= 0)
                        {
                            if (endCoef * endCoef >= beginCoef && beginAngle != endAngle)
                            {
                                amountToHave = (int)Math.Ceiling(endMonth * (endCoef * endCoef / beginCoef));
                            }
                            else if (endCoef * endCoef < beginCoef)
                            {
                                amountToHave = endMonth + (int)Math.Ceiling(endAngle * (endCoef * endCoef / beginCoef));
                            }
                            else
                            {
                                amountToHave = 2 * endMonth - middleMonth;
                            }
                        }
                        else if (beginAngle < 0 && endAngle < 0)
                        {
                            amountToHave = 2 * endMonth - middleMonth;
                        }
                        else
                        {
                            int avg = (int)Math.Ceiling(Convert.ToDouble((endMonth + middleMonth + beginMonth)) / 3);
                            amountToHave = Math.Max(avg, endMonth);
                        }


                        if (amountToHave < 0 && endMonth > 0)
                        {
                            amountToHave = 10;
                        }
                        else if (endMonth == 0)
                        {
                            amountToHave = 0;
                        }

                        int amountToBuy = cur_amount < amountToHave ? amountToHave - cur_amount : 0;

                        drugsPurchase.Add(
                            new Tuple<int, string, int, int, int, int, int>(id, name, beginMonth, middleMonth, endMonth, amountToBuy, amountToHave));
                    }
                }
            }

            listOfAnalys = drugsPurchase.Select(x => new DrugAnalysis(x.Item1, x.Item2, x.Item6, x.Item7)).ToList();

            AutomatizationTable.ItemsSource = listOfAnalys;
            currentTable = "";
            queriesDictionary = new Dictionary<string, string>
            {
                { "Drugs", "CALL GetDrugsInfo()" },
                { "History of sales", $"SELECT historyofsales.purchaseID,drugs.Drug_name AS Препарат," +
                $" historyofsales.Amount_of_purchased_product AS 'Количество приобретенного продукта', " +
                $"historyofsales.Date_of_purchase AS 'Дата покупки', " +
                $"historyofsales.Login AS Логин, " +
                $"historyofsales.Price as 'Цена' " +
                $"FROM (historyofsales INNER JOIN drugs ON historyofsales.DrugID = drugs.DrugID)" }
            };
            chart.ChartAreas.Add(new ChartArea("Default"));
            Series series = new Series("Series1");
            series.ChartType = SeriesChartType.Line;
            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart.Series.Add(series);
            chart.Series["Series1"].ChartArea = "Default";


            chart2.ChartAreas.Add(new ChartArea("Default1"));
            Series series1 = new Series("Series2");
            series1.ChartType = SeriesChartType.Pie;
            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart2.Series.Add(series1);
            chart2.Series["Series2"].ChartArea = "Default1";
            Title title = chart2.Titles.Add("Топ-5 производителей за все время");
            title.Font = new Font("Arial", 20);
            chart2.Legends.Add(new Legend("Legend2"));
            // Set Docking of the Legend chart to the Default Chart Area.
            chart2.Legends["Legend2"].DockedToChartArea = "Default1";
            chart2.Legends["Legend2"].Docking = Docking.Left;
            chart2.Legends["Legend2"].Font = new Font("Arial", 20);
            // Assign the legend to Series1.
            chart2.Series["Series2"].Legend = "Legend2";
            chart2.Series["Series2"].IsVisibleInLegend = true;

            chart3.ChartAreas.Add(new ChartArea("Default2"));
            Series series2 = new Series("Series3");
            series2.ChartType = SeriesChartType.Pie;
            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart3.Series.Add(series2);
            chart3.Series["Series3"].ChartArea = "Default2";
            Title title1 = chart3.Titles.Add("Топ-5 продуктов за все время");
            title1.Font = new Font("Arial", 20);
            chart3.Legends.Add(new Legend("Legend3"));
            // Set Docking of the Legend chart to the Default Chart Area.
            chart3.Legends["Legend3"].DockedToChartArea = "Default2";
            chart3.Legends["Legend3"].Docking = Docking.Left;
            chart3.Legends["Legend3"].Font = new Font("Arial", 20);
            // Assign the legend to Series1.
            chart3.Series["Series3"].Legend = "Legend3";
            chart3.Series["Series3"].IsVisibleInLegend = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //List<Drug> listOfDrugs = Drug.GetAllDrugs();

            //Random random = new Random();
            //for (int i = 0; i < 500; i++)
            //{
            //    int number = random.Next() % listOfDrugs.Count;
            //    int amount = random.Next() % 50 + 1;
            //    string wprice = "";

            //    if (listOfDrugs[number].WPricesList.Count > 0)
            //    {
            //        int j = 0;
            //        while (j < listOfDrugs[number].WPricesList.Count
            //            && amount >= listOfDrugs[number].WPricesList[j].Minimal_amount_of_product)
            //        {
            //            j++;
            //        }
            //        if (j != 0)
            //        {
            //            wprice = listOfDrugs[number].WPricesList[j - 1].Price;
            //        }
            //    }
            //    if (wprice != String.Empty)
            //    {
            //        List<Tuple<Drug, int, string>> d = new List<Tuple<Drug, int, string>>() {
            //            new Tuple<Drug, int, string>(listOfDrugs[number], amount, wprice)
            //        };
            //        Drug.AddPurchase(d, "newuser");
            //    }
            //    else
            //    {
            //        List<Tuple<Drug, int, string>> d = new List<Tuple<Drug, int, string>>() {
            //            new Tuple<Drug, int, string>(listOfDrugs[number], amount, listOfDrugs[number].Retail_price)
            //        };
            //        Drug.AddPurchase(d, "newuser");
            //    }
            //}
        }

        private void DrugsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "Drugs";
            Search.Visibility = Visibility.Visible;
            string sqlQ = queriesDictionary[currentTable];
            Logic.ShowTable(table, sqlQ);
        }

        private void ManufacturersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            List<Tuple<int, string>> list = Manufacturer.GetTopManufacturers();
            chart2.Series["Series2"].Points.DataBindXY(
                list.Select(x=>x.Item2).ToArray(), list.Select(x => x.Item1).ToArray());
            chart2.Series["Series2"].IsValueShownAsLabel = true;
            wfh2.Visibility = Visibility.Visible;
            Logic.ShowTable(table, "CALL GetManufacturersInfo();");
        }

        private void HistoryOfSalesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "History of sales";
            string sqlQ = queriesDictionary[currentTable];
            Logic.ShowTable(table, sqlQ);
            NumberOfPurchases.Text = $"Количество покупок: {table.Items.Count}";
            NumberOfPurchases.Visibility = Visibility.Visible;
            HistoryDatePicker.Visibility = Visibility.Visible;
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "";
        }

        private void Search_KeyUp(object sender, KeyEventArgs e)
        {
            string SearchKey = (sender as TextBox).Text;
            if (currentTable == "Drugs")
            {
                Logic.ShowTable(table, $"CALL SearchDrugInfo('{SearchKey}')");
            }
            else if (currentTable == "ManufacturersInfo")
            {
                Logic.ShowTable(table, $"CALL SearchManufacturer('{SearchKey}')");
            }
            else if (currentTable == "Symptoms")
            {
                Logic.ShowTable(table, $"CALL SearchSymptom('{SearchKey}')");
            }
            else if (currentTable == "Countries")
            {
                Logic.ShowTable(table, $"CALL SearchCountry('{SearchKey}')");
            }
            else if (currentTable == "Manufacturers") {
                Logic.ShowTable(table, $"CALL SearchManufacturerInfo('{SearchKey}')");
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "Поиск...";
        }

        private void SymptomDataControl_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "Symptoms";
            Logic.ShowTable(table, "CALL GetSymptoms()");
            Search.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void DrugDataControl_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "Drugs";
            Logic.ShowTable(table, "CALL GetDrugsInfo()");
            Search.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void ManufacturerDataControl_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "Manufacturers";
            Logic.ShowTable(table, "CALL GetManufacturers()");

            Search.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void CountryDataControl_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            currentTable = "Countries";
            Logic.ShowTable(table, "CALL GetCountries()");
            Search.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable == "Countries") {
                EditCountryWindow editCountry = new EditCountryWindow(table);
                editCountry.ShowDialog();
            }
            else if (currentTable == "Manufacturers") {
                EditManufacturerWindow editManufacturer = new EditManufacturerWindow(table);
                editManufacturer.ShowDialog();
            }
            else if (currentTable == "Drugs") {
                EditDrugWindow editDrug = new EditDrugWindow(table);
                editDrug.ShowDialog();
            }
            else {
                EditSymptomWindow editSymptom = new EditSymptomWindow(table);
                editSymptom.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (table.SelectedIndex != -1)
            {
                DataRowView row = (DataRowView)table.SelectedItems[0];
                int id = Convert.ToInt32(row.Row.ItemArray[0]);
                if (currentTable == "Countries")
                {
                    Logic.DeleteInformation($"CALL DeleteCountry({id})");
                    Logic.ShowTable(table, "CALL GetCountries()");
                }
                else if (currentTable == "Manufacturers")
                {
                    Logic.DeleteInformation($"CALL DeleteManufacturer({id})");
                    Logic.ShowTable(table, "CALL GetManufacturers()");
                }
                else if (currentTable == "Drugs")
                {
                    Logic.DeleteInformation($"CALL DeleteDrug({id})");
                    Logic.ShowTable(table, "CALL GetDrugsInfo()");
                }
                else
                {
                    Logic.DeleteInformation($"CALL DeleteSymptom({id})");
                    Logic.ShowTable(table, "CALL GetSymptoms()");
                }
            }
            else {
                MessageBox.Show("Для удаления выберите элемент таблицы");
            }
        }
        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)table.SelectedItems[0];
            int id = Convert.ToInt32(row.Row.ItemArray[0]);
            if (currentTable == "Countries")
            {
                EditCountryWindow editCountry = new EditCountryWindow(id, table);
                editCountry.ShowDialog();
            }
            else if (currentTable == "Manufacturers")
            {
                EditManufacturerWindow editManufacturer = new EditManufacturerWindow(id, table);
                editManufacturer.ShowDialog();
            }
            else if (currentTable == "Drugs")
            {
                EditDrugWindow editDrug = new EditDrugWindow(id, table);
                editDrug.ShowDialog();
            }
            else
            {
                EditSymptomWindow editSymptom = new EditSymptomWindow(id, table);
                editSymptom.ShowDialog();
            }
        }

        private void ManufacturerReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            wfh.Visibility = Visibility.Visible;

            
            // добавим данные линии
            List<int> quan = new List<int>();
            List<string> dates = new List<string>();
            using (MySqlConnection connection = new MySqlConnection("server = localhost; user id = root; persistsecurityinfo=True;database=drugstore;password=2699nes24"))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("CALL GetMonthSalesStats();", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int q = reader.GetInt32(0);
                        string month = reader.GetString(1);
                        quan.Add(q);
                        dates.Add(month);
                    }
                }
            }
            table.Visibility = Visibility.Collapsed;
            
            chart.Series["Series1"].Points.DataBindXY(dates, quan);
            chart.Series["Series1"].IsValueShownAsLabel = true;
        }

        private void IncomeReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            Logic.ShowTable(table,
                $"SELECT drugs.DrugID, drugs.Drug_name as 'Название', " +
                $"COUNT(historyofsales.PurchaseID) as 'Количество покупок', " +
                $"SUM(historyofsales.Price * historyofsales.Amount_of_purchased_product) - drugs.Wholesale_Price * SUM(historyofsales.Amount_of_purchased_product) as 'Прибыль', " +
                $"SUM(historyofsales.Amount_of_purchased_product * historyofsales.Price) as 'Доход', " +
                $"drugs.Wholesale_Price* SUM(historyofsales.Amount_of_purchased_product) as 'Затраты', " +
                $"drugs.Wholesale_Price as 'Цена закупки', " +
                $"SUM(historyofsales.Amount_of_purchased_product) as 'Количество проданного товара' " +
                $"FROM(drugs JOIN historyofsales ON drugs.DrugID = historyofsales.DrugID) " +
                $"WHERE MONTH(historyofsales.Date_of_purchase) = MONTH(NOW()) " +
                $"AND YEAR(historyofsales.Date_of_purchase) = YEAR(NOW())" +
                $"GROUP BY drugs.DrugID " +
                $"ORDER BY drugs.Drug_name;");
            DateComboBox.SelectedIndex = 0;
            CalculateIncome();
            DateComboBox.Visibility = Visibility.Visible;
            Income.Visibility = Visibility.Visible;
            GetReport.Visibility = Visibility.Visible;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Logic.ShowTable(table, queriesDictionary["History of sales"] + 
                $" WHERE historyofsales.Date_of_purchase " +
                $"BETWEEN '{((DateTime)HistoryDatePicker.SelectedDate).ToString("yyyy-MM-dd")}' AND now()");
            NumberOfPurchases.Text = $"Количество покупок: {table.Items.Count}";
        }

        private void DateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateComboBox.SelectedIndex == 0) {
                Logic.ShowTable(table, Logic.IncomeRequest(
                    $"WHERE MONTH(historyofsales.Date_of_purchase) = MONTH(NOW()) " +
                    $"AND YEAR(historyofsales.Date_of_purchase) = YEAR(NOW())"));
                CalculateIncome();
            }
            else if (DateComboBox.SelectedIndex == 1) {
                Logic.ShowTable(table, Logic.IncomeRequest(
                    $"WHERE YEAR(historyofsales.Date_of_purchase) = YEAR(NOW()) " +
                    $"AND WEEK(historyofsales.Date_of_purchase, 1) = WEEK(NOW(), 1)"));
                CalculateIncome();
            }
            else if (DateComboBox.SelectedIndex == 2)
            {
                Logic.ShowTable(table, Logic.IncomeRequest(
                    $"WHERE MONTH(historyofsales.Date_of_purchase) = MONTH(DATE_ADD(NOW(), INTERVAL -1 MONTH)) " +
                    $"AND YEAR(historyofsales.Date_of_purchase) = YEAR(NOW())"));
                CalculateIncome();
            }
        }

        private void Collapse() {
            table.Visibility = Visibility.Visible;
            Search.Visibility = Visibility.Collapsed;
            DateComboBox.Visibility = Visibility.Collapsed;
            HistoryDatePicker.Visibility = Visibility.Collapsed;
            Income.Visibility = Visibility.Collapsed;
            AddButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            GetReport.Visibility = Visibility.Collapsed;
            NumberOfPurchases.Visibility = Visibility.Collapsed;
            wfh.Visibility = Visibility.Collapsed;
            wfh2.Visibility = Visibility.Collapsed;
            wfh3.Visibility = Visibility.Collapsed;
            AutomatizationTable.Visibility = Visibility.Collapsed;
        }
        private void CalculateIncome() {
            if (table.Items.Count > 0)
            {
                DataRowView row = (DataRowView)table.Items[0];
                double income = 0;
                for (int i = 0; i < table.Items.Count; i++)
                {
                    row = (DataRowView)table.Items[i];
                    income += Convert.ToDouble(row.Row.ItemArray[3]);
                }
                Income.Text = $"Суммарная прибыль составляет: {income.ToString()}";
            }
            else {
                Income.Text = $"Суммарная прибыль составляет: 0";
            }
        }

        private void GetReport_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Do you want to create a new file?",
              "WonderWord",
              MessageBoxButton.YesNo,
              MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:

                    Word.Application app = new Word.Application();
                    Word.Document doc = app.Documents.Add(Visible: true);
                    Word.Range r = doc.Range();
                    Word.Table t = doc.Tables.Add(r, table.Items.Count + 1, table.Columns.Count-1);
                    t.Borders.Enable = 1;
                    foreach (Word.Row row in t.Rows)
                    {
                        foreach (Word.Cell cell in row.Cells)
                        {
                            if (cell.RowIndex == 1)
                            {
                                cell.Range.Text = table.Columns[cell.ColumnIndex].Header.ToString();
                                cell.Range.Bold = 1;
                                cell.Range.Font.Name = "verdana";
                                cell.Range.Font.Size = 10;

                                cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                            else
                            {
                                cell.Range.Text = (table.Items[cell.RowIndex - 2] as DataRowView)
                                    .Row.ItemArray[cell.ColumnIndex].ToString();
                            }
                        }
                    }
                    doc.Save();
                    //app.Documents.Open(@"C:\Users\Андрей\Documents\Doc1.docx");
                    try
                    {
                        doc.Close();
                        app.Quit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Collapse();
            table.Visibility = Visibility.Collapsed;
            AutomatizationTable.Visibility = Visibility.Visible;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            ((sender as System.Windows.Controls.Image).Parent as StackPanel).Background = System.Windows.Media.Brushes.AliceBlue;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            ((sender as System.Windows.Controls.Image).Parent as StackPanel).Background = System.Windows.Media.Brushes.White;
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32((((sender as System.Windows.Controls.Image).Parent as StackPanel).Children[1] as TextBlock).Text);
            DrugAnalysis drugAnalysis = listOfAnalys.Where(x => x.ID == id).First();
            PurchasesOfDrug purchasesOfDrug = new PurchasesOfDrug(id, drugAnalysis);
            purchasesOfDrug.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Collapse();
            table.Visibility = Visibility.Collapsed;
            List<Tuple<int, string>> list = Drug.GetTopDrugs();
            chart3.Series["Series3"].Points.DataBindXY(
                list.Select(x => x.Item2).ToArray(), list.Select(x => x.Item1).ToArray());
            chart3.Series["Series3"].IsValueShownAsLabel = true;
            wfh3.Visibility = Visibility.Visible;
        }
    }
}
