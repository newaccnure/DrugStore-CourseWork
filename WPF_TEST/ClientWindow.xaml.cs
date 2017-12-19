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
using System.Data.OleDb;
using System.Data;

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private int currentPage = 0;
        private int pageSize = 12;
        private int numberOfDrugs = 0;
        List<Manufacturer> dictionaryManufacturers = new List<Manufacturer>();
        List<Symptom> dictionarySymptoms = new List<Symptom>();
        List<Country> dictionaryCountries = new List<Country>();
        private int numberOfCheckedCountries = 0;
        private string orderToSort = "drugs.DrugID";
        public List<Tuple<Drug, int, string>> basket = new List<Tuple<Drug, int, string>>();
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void OnEnterContent(object sender, MouseEventArgs e)
        {
            StackPanel s = (sender as StackPanel);
            s.Background = Brushes.Aqua;
            s.Cursor = Cursors.Hand;
        }
        private void OnLeaveContent(object sender, MouseEventArgs e)
        {
            StackPanel s = (sender as StackPanel);
            s.Background = Brushes.White;
            s.Cursor = Cursors.Arrow;
        }
        private void OnClickContent(object sender, MouseEventArgs e)
        {
            StackPanel s = (sender as StackPanel);
            s.Background = Brushes.White;

            string Login = "";
            if (Registration.Header.ToString() != "Зарегистрироваться") {
                Login = Registration.Header.ToString().Substring(Registration.Header.ToString().IndexOf(',') + 2);
            }
            BuyProductWindow buyProductWindow = new BuyProductWindow(
                Convert.ToInt32((s.Children[s.Children.Count - 1] as Label).Content), basket, Login);
            this.Visibility = Visibility.Hidden;
            buyProductWindow.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow(Authorization, Registration);
            registrationWindow.ShowDialog();
        }

        private void Authorization_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow(Authorization, Registration)
            {
                Owner = this
            };
            authorizationWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dictionaryManufacturers = Manufacturer.GetManufacturers();
            dictionarySymptoms = Symptom.GetSymptoms();
            dictionaryCountries = Country.GetCountries();
            
            foreach (Manufacturer manufacturer in dictionaryManufacturers)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = manufacturer.ManufacturerName
                };
                ManufacturerScrollViewer.Children.Add(checkBox);
            }
            foreach (Country country in dictionaryCountries)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = country.CountryName
                };
                checkBox.Click += CountryCheckBoxChange;
                CountryScrollViewer.Children.Add(checkBox);
            }
            foreach (Symptom symptom in dictionarySymptoms)
            {
                Button button = new Button();
                button.Click += SymptomButton_Click;
                button.Content = symptom.SymptomName;
                SymptomScrollViewer.Children.Add(button);
            }

            //Randoming purchases

            //List<Drug> listOfDrugs = Drug.GetAllDrugs();

            //Random random = new Random();
            //for (int i = 0; i < 100; i++)
            //{
            //    int number = random.Next() % listOfDrugs.Count;
            //    int amount = random.Next() % 10 + 1;
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

            ShowContent(FormFilterRequest());
            
        }

        private void SymptomButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);
            foreach (Symptom s in dictionarySymptoms) {
                if (s.SymptomName == button.Content.ToString()) {
                    ShowContent(FormFilterRequest() + $" AND symptoms.SymptomID={s.SymptomID} ");
                    break;
                }
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberOfDrugs < pageSize) {
                return;
            }
            currentPage++;
            ShowContent(FormFilterRequest());
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 0) return;
            currentPage--;
            ShowContent(FormFilterRequest());
        }

        private void CountryCheckBoxChange(object sender, RoutedEventArgs e)
        {
            CheckBox countryCheckBox = (sender as CheckBox);
            string country = countryCheckBox.Content.ToString();
            int countryID = -1;
            for (int i = 0; i < CountryScrollViewer.Children.Count; i++) {
                if ((CountryScrollViewer.Children[i] as CheckBox).Content.ToString() == country) {
                    countryID = dictionaryCountries[i].CountryID;
                    break;
                }
            }
            if ((bool)countryCheckBox.IsChecked)
            {
                for (int j = 0; j < dictionaryManufacturers.Count; j++)
                {
                    if (numberOfCheckedCountries == 0)
                    {
                        ManufacturerScrollViewer.Children[j].Visibility = Visibility.Collapsed;
                    }
                    if (dictionaryManufacturers[j].CountryID == countryID)
                    {
                        ManufacturerScrollViewer.Children[j].Visibility = Visibility.Visible;
                    }
                    else if (numberOfCheckedCountries == 0)
                    {
                        (ManufacturerScrollViewer.Children[j] as CheckBox).IsChecked = false;
                    }
                }
                numberOfCheckedCountries++;
            }
            else
            {
                numberOfCheckedCountries--;
                for (int j = 0; j < dictionaryManufacturers.Count; j++)
                {
                    if (dictionaryManufacturers[j].CountryID == countryID)
                    {
                        ManufacturerScrollViewer.Children[j].Visibility = Visibility.Collapsed;
                        (ManufacturerScrollViewer.Children[j] as CheckBox).IsChecked = false;
                    }
                    if (numberOfCheckedCountries == 0)
                        ManufacturerScrollViewer.Children[j].Visibility = Visibility.Visible;
                }
            }

        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(FormFilterRequest());
        }

        private void CheapPriceButton_Click(object sender, RoutedEventArgs e)
        {
            orderToSort = " drugs.Retail_price ";
            currentPage = 0;
            ClearButtons();
            CheapPriceButton.Background = Brushes.Orange;
            ShowContent(FormFilterRequest());
        }

        private void ExpensivePriceButton_Click(object sender, RoutedEventArgs e)
        {
            orderToSort = " drugs.Retail_price DESC ";
            currentPage = 0;
            ClearButtons();
            ExpensivePriceButton.Background = Brushes.Orange;
            ShowContent(FormFilterRequest());
        }

        public string FormFilterRequest() {
            string result = "";
            int lowerPrice = Int32.MinValue;
            int higherPrice = Int32.MaxValue;
            if (LowerPrice.Text != String.Empty && LowerPrice.Text!="От") {
                lowerPrice = Int32.Parse(LowerPrice.Text);
            }
            if (HigherPrice.Text != String.Empty && HigherPrice.Text != "До")
            {
                higherPrice = Int32.Parse(HigherPrice.Text);
            }
            result += $"AND (drugs.Retail_price BETWEEN {lowerPrice} AND {higherPrice})";

            List<Manufacturer> filterManufacturersList = new List<Manufacturer>();
            for (int i = 0; i < ManufacturerScrollViewer.Children.Count; i++) {
                if ((bool)(ManufacturerScrollViewer.Children[i] as CheckBox).IsChecked) {
                    filterManufacturersList.Add(dictionaryManufacturers[i]);
                }
            }

            string manufacturers = "";
            string countries = "";
            if (filterManufacturersList.Count == 0)
            {
                List<Country> filterCountriesList = new List<Country>();
                for (int i = 0; i < CountryScrollViewer.Children.Count; i++)
                {
                    if ((bool)(CountryScrollViewer.Children[i] as CheckBox).IsChecked)
                    {
                        filterCountriesList.Add(dictionaryCountries[i]);
                    }
                }
                if (filterCountriesList.Count != 0)
                {
                    countries += $"(countries.CountryID={filterCountriesList[0].CountryID}";
                    for (int i=1;i< filterCountriesList.Count;i++) {
                        countries += $" OR countries.CountryID={filterCountriesList[i].CountryID}";
                    }
                    countries += ")";
                }
            }
            else {
                manufacturers += $"(manufacturers.ManufacturerID={filterManufacturersList[0].ManufacturerID}";
                for (int i = 1; i < filterManufacturersList.Count; i++)
                {
                    manufacturers += $" OR manufacturers.ManufacturerID={filterManufacturersList[i].ManufacturerID}";
                }
                manufacturers += ")";
            }

            if (manufacturers != String.Empty) {
                result += $" AND {manufacturers} ";
            }
            if (countries != String.Empty) {
                result += $" AND {countries} ";
            }

            if ((bool)AvailableDrugCheckBox.IsChecked) {
                result += "AND drugs.Current_amount > 0";
            }

            return result;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowContent(FormFilterRequest());
        }

        public string FormSearchRequest() {
            string result = $" Drug_name LIKE '{SearchBox.Text}%'";
            currentPage = 0;
            pageSize = 12;
            return result;
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LowerPrice.Text = "От";
            HigherPrice.Text = "До";

            foreach (CheckBox c in CountryScrollViewer.Children)
            {
                c.IsChecked = false;
            }
            ManufacturerScrollViewer.Children.RemoveRange(0, ManufacturerScrollViewer.Children.Count);
            
            AvailableDrugCheckBox.IsChecked = false;

            foreach (Manufacturer manufacturer in dictionaryManufacturers)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = manufacturer.ManufacturerName
                };
                ManufacturerScrollViewer.Children.Add(checkBox);
            }
            currentPage = 0;
            numberOfCheckedCountries = 0;
            orderToSort = "drugs.DrugID";
            ClearButtons();
            ShowContent(FormFilterRequest());
        }

        private void RatingSortButton_Click(object sender, RoutedEventArgs e)
        {
            orderToSort = "Rating";
            ClearButtons();
            RatingSortButton.Background = Brushes.Orange;
            currentPage = 0;
            ShowContent(FormFilterRequest());
        }

        private void ClearButtons() {
            RatingSortButton.Background = Brushes.White;
            ExpensivePriceButton.Background = Brushes.White;
            CheapPriceButton.Background = Brushes.White;
        }

        private void FillWithDrugs(List<Drug> drugsList) {
            foreach (Drug drug in drugsList)
            {
                StackPanel stackPanel = new StackPanel();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(drug.URL_photo, UriKind.Absolute);
                bitmap.EndInit();
                Image image = new Image
                {
                    Source = bitmap,
                    Height = 300,
                    Width = 300
                };
                TextBlock name = new TextBlock
                {
                    Text = drug.Drug_name,
                    TextWrapping = TextWrapping.Wrap
                };
                TextBlock price = new TextBlock
                {
                    Text = drug.Retail_price.ToString()
                };
                TextBlock presence = new TextBlock();
                if (drug.Current_amount > 0)
                {
                    presence.Text = "В наличии";
                    presence.Foreground = Brushes.Green;
                }
                else
                {
                    presence.Text = "Нет в наличии";
                    presence.Foreground = Brushes.Red;
                }
                Label labelID = new Label
                {
                    Content = drug.DrugID,
                    Visibility = Visibility.Collapsed
                };
                stackPanel.MouseEnter += OnEnterContent;
                stackPanel.MouseLeftButtonDown += OnClickContent;
                stackPanel.MouseLeave += OnLeaveContent;
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(name);
                stackPanel.Children.Add(price);
                stackPanel.Children.Add(presence);
                stackPanel.Children.Add(labelID);
                content.Children.Add(stackPanel);
            }
        }

        private void ShowContent(string filter) {
            content.Children.RemoveRange(0, content.Children.Count);
            List<Drug> drugsList = new List<Drug>();
            if (orderToSort != "Rating")
            {
                drugsList = Drug.GetDrugInfoList(filter,
                        $" Drug_name LIKE '{SearchBox.Text}%'", currentPage, pageSize, orderToSort);
            }
            else {
                drugsList = Drug.GetRatingSortedDrugList(filter,
                    $" Drug_name LIKE '{SearchBox.Text}%'", currentPage, pageSize);
            }
            numberOfDrugs = drugsList.Count;
            FillWithDrugs(drugsList);
        }

        private void CartOpenLabelClick(object sender, MouseButtonEventArgs e)
        {
            string Login = "";
            if (Registration.Header.ToString() != "Зарегистрироваться")
            {
                Login = Registration.Header.ToString().Substring(Registration.Header.ToString().IndexOf(',') + 2);
            }
            CartWindow cartWindow = new CartWindow(basket, Login);
            cartWindow.ShowDialog();
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}
