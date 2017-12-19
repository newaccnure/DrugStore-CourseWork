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
    /// Логика взаимодействия для EditManufacturerWindow.xaml
    /// </summary>
    public partial class EditManufacturerWindow : Window
    {
        private int ManufacturerID;
        private DataGrid DataGrid;
        List<Country> dictionaryCountries = new List<Country>();
        public EditManufacturerWindow(DataGrid dataGrid)
        {
            InitializeComponent();
            DataGrid = dataGrid;
            dictionaryCountries = Country.GetCountries().OrderBy(x=>x.CountryName).ToList();
            foreach (Country country in dictionaryCountries) {
                CountriesComboBox.Items.Add(country.CountryName);
            }
        }

        public EditManufacturerWindow(int manufacturerID, DataGrid dataGrid) : this(dataGrid) {
            ManufacturerID = manufacturerID;
            AddButton.Content = "Изменить";
            Manufacturer manufacturer = Manufacturer.GetManufacturerById(manufacturerID);
            CountriesComboBox.SelectedValue = dictionaryCountries.Where(x=>x.CountryID == manufacturer.CountryID).Select(x => x.CountryName).First();
            ManufacturerTextBox.Text = manufacturer.ManufacturerName;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddButton.Content.ToString() == "Добавить")
            {
                //Add
                if (ManufacturerTextBox.Text != String.Empty)
                {
                    if (CountriesComboBox.SelectedIndex != -1)
                    {
                        string addCountryQuery = 
                            $"CALL InsertManufacturer('{ManufacturerTextBox.Text}',{dictionaryCountries[CountriesComboBox.SelectedIndex].CountryID})";
                        Logic.InsertInformation(addCountryQuery);
                        Logic.ShowTable(DataGrid, "CALL GetManufacturers()");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Выберите страну-производителя");
                    }
                }
                else
                {
                    MessageBox.Show("Введите название производителя");
                }
            }
            else {
                //Change
                if (CountriesComboBox.SelectedIndex != -1)
                {
                    Manufacturer.UpdateManufacturer(ManufacturerID, ManufacturerTextBox.Text,
                        dictionaryCountries[CountriesComboBox.SelectedIndex].CountryID);
                    Logic.ShowTable(DataGrid, "CALL GetManufacturers()");
                    Close();
                }
                else
                {
                    MessageBox.Show("Введите название производителя");
                }
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
