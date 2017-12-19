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
using System.Windows.Shapes;
using eisiWare;

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для BuyProductWindow.xaml
    /// </summary>
    public partial class BuyProductWindow : Window
    {
        private Drug ShowedDrug;
        private List<Tuple<Drug, int, string>> Basket;
        private string Login = "";
        public BuyProductWindow()
        {
            InitializeComponent();
        }
        public BuyProductWindow(int drugID, 
            List<Tuple<Drug, int, string>> basket, string login) : this() {
            Login = login;
            ShowedDrug = new Drug(drugID);
            Basket = basket;
            NumericUpDown numericUpDown = new NumericUpDown
            {
                FontSize = 25,
                Height = 40,
                Value = 1,
                Name = "numericUpDown"
            };
            Grid.SetColumn(numericUpDown,0);
            Grid.SetRow(numericUpDown, 0);
            emptyGrid.Children.Add(numericUpDown);

            DrugNameLabel.Content = $"{ShowedDrug.Drug_name}, {ShowedDrug.Weight_Volume}";
            PriceLabel.Content = $"{ShowedDrug.Retail_price} грн.";
            Availability.Content = ShowedDrug.Current_amount > 0 ? "В наличии" : "Нет в наличии";
            Availability.Foreground = ShowedDrug.Current_amount > 0 ? Brushes.Green : Brushes.Red;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(ShowedDrug.URL_photo, UriKind.Absolute);
            bitmap.EndInit();
            DrugImage.Source = bitmap;

            
            Label label = new Label();
            label.Content += "Производители: ";
            label.FontSize = 16;
            for (int i = 0; i < ShowedDrug.ManufacturersList.Count; i++)
            {
                label.Content += ShowedDrug.ManufacturersList[i].ManufacturerName;
                if (i != ShowedDrug.ManufacturersList.Count - 1) {
                    label.Content += ", ";
                }
            }
            ManufacturersChar.Children.Add(label);
            ShowedDrug.ManufacturersList.Distinct(new ManufacturerComparer());
            List<Country> list = Country.GetCountries();

            Label CountryLabel = new Label
            {
                FontSize = 16
            };
            if (ShowedDrug.ManufacturersList.Count > 1)
            {
                CountryLabel.Content += "Страны-производители: ";
            }
            else {
                CountryLabel.Content += "Страна-производитель: ";
            }
            for (int i = 0; i < ShowedDrug.ManufacturersList.Count; i++)
            {
                string countryname = list
                    .Where(x => x.CountryID == ShowedDrug.ManufacturersList[i].CountryID)
                    .Select(x => x.CountryName)
                    .First()
                    .ToString();

                CountryLabel.Content += countryname;
                if (i != ShowedDrug.ManufacturersList.Count - 1)
                {
                    CountryLabel.Content += ", ";
                }
            }
            CountriesChar.Children.Add(CountryLabel);

            if (ShowedDrug.WPricesList.Count > 0) {
                ShowedDrug.WPricesList = ShowedDrug.WPricesList.OrderBy(x => x.Minimal_amount_of_product).ToList();
                foreach (WholesalePriceClient wp in ShowedDrug.WPricesList) {
                    TextBlock textBlock = new TextBlock
                    {
                        FontSize = 18,
                        Text = $"При заказе от {wp.Minimal_amount_of_product} шт.\n {wp.Price} грн."
                    };
                    WholesalePrices.Children.Add(textBlock);
                }
            }

            ApplicationTextBox.Text = ShowedDrug.Application;
            WarningTextBox.Text = ShowedDrug.Warning;
            DescriptionTextBox.Text = ShowedDrug.Description;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var element = emptyGrid.Children.Cast<UIElement>().
                First(el => Grid.GetColumn(el) == 0 && Grid.GetRow(el) == 0);
            NumericUpDown numericUp = (element as NumericUpDown);

            int boughtAmount;
            if (!Int32.TryParse(numericUp.Value.ToString(), out boughtAmount))
            {
                MessageBox.Show("Проверьте правильность ввода данных");
            }
            else
            {
                string wprice = "";

                if (ShowedDrug.WPricesList.Count > 0)
                {
                    int i = 0;
                    while (i < ShowedDrug.WPricesList.Count
                        && boughtAmount >= ShowedDrug.WPricesList[i].Minimal_amount_of_product)
                    {
                        i++;
                    }
                    if (i != 0)
                    {
                        wprice = ShowedDrug.WPricesList[i - 1].Price;
                    }
                }
                if (boughtAmount < ShowedDrug.Current_amount)
                {
                    if (wprice != String.Empty)
                    {
                        Basket.Add(new Tuple<Drug, int, string>(ShowedDrug, boughtAmount, wprice));
                    }
                    else
                    {
                        Basket.Add(new Tuple<Drug, int, string>(ShowedDrug,
                            boughtAmount, ShowedDrug.Retail_price));
                    }
                    CartWindow cartWindow = new CartWindow(Basket, Login);
                    Hide();
                    cartWindow.ShowDialog();
                    Close();
                }
                else {
                    MessageBox.Show($"На данный момент у нас не имеется " +
                        $"этот продукт в таком количестве {boughtAmount}");
                }
            }
        }
    }
}
