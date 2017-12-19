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

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для EditCountryWindow.xaml
    /// </summary>
    public partial class EditCountryWindow : Window
    {
        private int CountryID;
        private DataGrid DataGrid;
        public EditCountryWindow(DataGrid dataGrid)
        {
            InitializeComponent();
            DataGrid = dataGrid;
        }

        public EditCountryWindow(int countryID, DataGrid dataGrid) : this(dataGrid) {
            CountryID = countryID;
            AddButton.Content = "Изменить";
            CountryTextBox.Text = Country.GetCountryById(countryID);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddButton.Content.ToString() == "Добавить")
            {
                //Add
                if (CountryTextBox.Text != String.Empty)
                {
                    string addCountryQuery = $"CALL InsertCountry('{CountryTextBox.Text}')";
                    Logic.InsertInformation(addCountryQuery);
                    Logic.ShowTable(DataGrid, "CALL GetCountries()");
                    Close();
                }
                else {
                    MessageBox.Show("Введите название страны");
                }
            }
            else {
                //Change
                Country.UpdateCountry(CountryID, CountryTextBox.Text);
                Logic.ShowTable(DataGrid, "CALL GetCountries()");
                Close();
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
