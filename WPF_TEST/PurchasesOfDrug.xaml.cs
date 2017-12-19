using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для PurchasesOfDrug.xaml
    /// </summary>
    public partial class PurchasesOfDrug : Window
    {
        public PurchasesOfDrug()
        {
            InitializeComponent();
        }
        public PurchasesOfDrug(int drugid, DrugAnalysis drugAnalysis) : this() {
            List<Tuple<int, string>> list = Drug.GetSalesPerMonthByID(drugid);


            chart.ChartAreas.Add(new ChartArea("Default"));
            Series series = new Series("Series1");
            series.ChartType = SeriesChartType.Line;
            chart.Series.Add(series);
            chart.Series["Series1"].ChartArea = "Default";

            Series series1 = new Series("Series2");
            series1.ChartType = SeriesChartType.Line;
            chart.Series.Add(series1);
            chart.Series["Series1"].ChartArea = "Default";

            List<string> dates = new List<string>() {
                DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US"))+", "+ DateTime.Now.Year,
                DateTime.Now.AddMonths(1).ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US")) + ", " + DateTime.Now.AddMonths(1).Year
            };

            List<int> amounts = new List<int>() {
                list[list.Count - 1].Item1,
                drugAnalysis.AmountToHave
            };
            
            chart.Series["Series1"].Points.DataBindXY(list.Select(x=>x.Item2).ToArray(), list.Select(x => x.Item1).ToArray());
            chart.Series["Series1"].IsValueShownAsLabel = true;
            chart.Series["Series2"].Points.DataBindXY(dates, amounts);
            chart.Series["Series2"].Points[1].IsValueShownAsLabel = true;
            Title title = chart.Titles.Add("Уровень продаж по месяцам");
            title.Font = new Font("Arial", 20);
            chart.AlignDataPointsByAxisLabel();
        }
    }
}
