using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для EditDrugWindow.xaml
    /// </summary>
    public partial class EditDrugWindow : Window
    {
        private Drug EditedDrug = new Drug();
        private DataGrid DataGrid;
        private int DrugID = 0;
        private List<Symptom> dictionarySymptoms = new List<Symptom>();
        private List<Manufacturer> dictionaryManufacturers = new List<Manufacturer>();
        private string currentButtonName = "a";

        public EditDrugWindow(DataGrid dataGrid)
        {
            InitializeComponent();
            DataGrid = dataGrid;
            dictionarySymptoms = Symptom.GetSymptoms();
            dictionaryManufacturers = Manufacturer.GetManufacturers();
            foreach (Manufacturer manufacturer in dictionaryManufacturers)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = manufacturer.ManufacturerName;
                ManufacturerScrollViewer.Children.Add(checkBox);
            }
            foreach (Symptom symptom in dictionarySymptoms)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = symptom.SymptomName;
                SymptomScrollViewer.Children.Add(checkBox);
            }
        }

        public EditDrugWindow(int drugID, DataGrid dataGrid) : this(dataGrid) {
            EditedDrug = new Drug(drugID);
            DrugID = drugID;
            AddButton.Content = "Изменить";
            DrugNameTextBox.Text = EditedDrug.Drug_name;
            DrugWholesalePriceTextBox.Text = EditedDrug.WholesalePrice.ToString();
            RetailPriceTextBox.Text = EditedDrug.Retail_price.ToString();
            CurrentAmountTextBox.Text = EditedDrug.Current_amount.ToString();
            WeightVolumeTextBox.Text = EditedDrug.Weight_Volume;
            ApplicationTextBox.Document.Blocks.Add(new Paragraph(new Run(EditedDrug.Application)));
            DescriptionTextBox.Document.Blocks.Add(new Paragraph(new Run(EditedDrug.Description)));
            WarningTextBox.Document.Blocks.Add(new Paragraph(new Run(EditedDrug.Warning)));
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(EditedDrug.URL_photo, UriKind.Absolute);
            bitmap.EndInit();
            DrugImage.Source = bitmap;

            foreach (WholesalePriceClient wp in EditedDrug.WPricesList) {
                AddWPrice(wp.Price.ToString(),wp.Minimal_amount_of_product.ToString());
            }
            
            foreach (Manufacturer manufDrug in EditedDrug.ManufacturersList) {
                for (int i = 0; i < dictionaryManufacturers.Count; i++)
                {
                    if (dictionaryManufacturers[i].ManufacturerID == manufDrug.ManufacturerID) {
                        (ManufacturerScrollViewer.Children[i] as CheckBox).IsChecked = true;
                    }
                }
            }

            foreach (Symptom sympDrug in EditedDrug.SymptomsList)
            {
                for (int i = 0; i < dictionarySymptoms.Count; i++)
                {
                    if (dictionarySymptoms[i].SymptomID == sympDrug.SymptomID)
                    {
                        (SymptomScrollViewer.Children[i] as CheckBox).IsChecked = true;
                    }
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            bool add = true;
            double priceOfBought = 0;
            double retailPrice = 0;
            int amount = 0;
            double wprice = 0;
            int quan = 0;
            string name = "";
            List<WholesalePriceClient> list = new List<WholesalePriceClient>();
            DrugNameTextBox.Background = Brushes.White;
            DrugWholesalePriceTextBox.Background = Brushes.White;
            RetailPriceTextBox.Background = Brushes.White;
            CurrentAmountTextBox.Background = Brushes.White;
            WeightVolumeTextBox.Background = Brushes.White;
            SymptomLabel.Background = Brushes.White;
            ManufacturerLabel.Background = Brushes.White;
            if (DrugNameTextBox.Text == String.Empty)
            {
                DrugNameTextBox.Background = Brushes.Red;
                add = false;
            }
            else {
                name = DrugNameTextBox.Text;
            }
            if (!double.TryParse(DrugWholesalePriceTextBox.Text, out priceOfBought)) {
                MakeRed(DrugWholesalePriceTextBox, out add);
            }
            if (!double.TryParse(RetailPriceTextBox.Text, out retailPrice))
            {
                MakeRed(RetailPriceTextBox, out add);
            }
            if (!int.TryParse(CurrentAmountTextBox.Text, out amount)) {
                MakeRed(CurrentAmountTextBox, out add);
            }
            if (WeightVolumeTextBox.Text == String.Empty) {
                MakeRed(WeightVolumeTextBox, out add);
            }
            
            foreach (StackPanel s in WholesalePrices.Children) {
                if (!double.TryParse(((s.Children[0] as StackPanel).Children[1] as TextBox).Text, out wprice) || wprice <= 0) {
                    MakeRed(((s.Children[0] as StackPanel).Children[1] as TextBox), out add);
                }
                if (!Int32.TryParse(((s.Children[1] as StackPanel).Children[1] as TextBox).Text, out quan) || quan <= 0)
                {
                    MakeRed(((s.Children[1] as StackPanel).Children[1] as TextBox), out add);
                }
            }

            int i = 0;
            foreach (CheckBox c in SymptomScrollViewer.Children) {
                if ((bool)c.IsChecked) i++;
            }
            if (i == 0) {
                MakeRed(SymptomLabel, out add);
            }

            i = 0;
            foreach (CheckBox c in ManufacturerScrollViewer.Children)
            {
                if ((bool)c.IsChecked) i++;
            }
            if (i == 0)
            {
                MakeRed(ManufacturerLabel, out add);
            }

            Drug changedDrug = new Drug();

            if (add) {
                string path = (DrugImage.Source as BitmapImage).UriSource.AbsoluteUri;
                changedDrug.DrugID = DrugID;
                changedDrug.Application = new TextRange(
                    ApplicationTextBox.Document.ContentStart, 
                    ApplicationTextBox.Document.ContentEnd).Text;
                changedDrug.Current_amount = amount;
                changedDrug.Description= new TextRange(
                    DescriptionTextBox.Document.ContentStart,
                    DescriptionTextBox.Document.ContentEnd).Text;
                changedDrug.Drug_name = name;
                changedDrug.Retail_price = retailPrice.ToString().Replace(',','.');
                changedDrug.URL_photo = path;
                changedDrug.Warning = new TextRange(
                    WarningTextBox.Document.ContentStart,
                    WarningTextBox.Document.ContentEnd).Text;
                changedDrug.Weight_Volume = WeightVolumeTextBox.Text;
                changedDrug.WholesalePrice = priceOfBought.ToString().Replace(',', '.');

                foreach (StackPanel s in WholesalePrices.Children)
                {
                    WholesalePriceClient priceClient = new WholesalePriceClient();
                    priceClient.Price = (((s.Children[0] as StackPanel).Children[1] as TextBox).Text).Replace(',', '.');
                    priceClient.Minimal_amount_of_product = Int32.Parse(((s.Children[1] as StackPanel).Children[1] as TextBox).Text);
                    changedDrug.WPricesList.Add(priceClient);
                }

                for (int j = 0; j < SymptomScrollViewer.Children.Count; j++) {
                    if ((bool)(SymptomScrollViewer.Children[j] as CheckBox).IsChecked) {
                        changedDrug.SymptomsList.Add(dictionarySymptoms[j]);
                    }
                }

                for (int j = 0; j < ManufacturerScrollViewer.Children.Count; j++)
                {
                    if ((bool)(ManufacturerScrollViewer.Children[j] as CheckBox).IsChecked)
                    {
                        changedDrug.ManufacturersList.Add(dictionaryManufacturers[j]);
                    }
                }

                if (AddButton.Content.ToString() == "Добавить")
                {
                    //Add

                    Drug.AddDrug(changedDrug);
                }
                else
                {
                    //Change

                    Drug.UpdateDrug(changedDrug);
                }

                Logic.ShowTable(DataGrid, "CALL GetDrugsInfo()");
                Close();
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|gif files (*.gif)|*.gif";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(openFileDialog1.FileName, UriKind.RelativeOrAbsolute);
                        bitmap.EndInit();
                        DrugImage.Source = bitmap;
                    }
                }
            }
        }

        private void AddWPriceButton_Click(object sender, RoutedEventArgs e)
        {
            AddWPrice("", "");
        }

        private void AddWPrice(string price, string amount)
        {
            StackPanel horizontalStackPanel = new StackPanel();
            StackPanel stackPanelPrice = new StackPanel();
            Label labelPrice = new Label();
            TextBox textBoxPrice = new TextBox();
            StackPanel stackPanelQuantity = new StackPanel();
            Label labelQuantity = new Label();
            TextBox textBoxQuantity = new TextBox();
            Button button = new Button();
            button.Name = currentButtonName;
            horizontalStackPanel.Name = currentButtonName.Replace('a', 'b');
            currentButtonName += "a";
            Image image = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("C:\\Users\\Андрей\\Documents\\Visual Studio 2017\\Projects\\WPF_TEST\\WPF_TEST\\Icons\\minus.png", UriKind.Absolute);
            bitmap.EndInit();
            image.Source = bitmap;
            button.Content = image;
            button.Height = 26;
            button.Width = 24;
            button.Click += Button_Click1;
            labelPrice.Content = "Цена";
            labelQuantity.Content = "Количество";
            horizontalStackPanel.Orientation = Orientation.Horizontal;
            stackPanelPrice.Margin = new Thickness(10);
            stackPanelQuantity.Margin = new Thickness(10);
            textBoxPrice.Width = 50;
            textBoxQuantity.Width = 50;
            textBoxPrice.Text = price;
            textBoxQuantity.Text = amount;
            stackPanelPrice.Children.Add(labelPrice);
            stackPanelPrice.Children.Add(textBoxPrice);
            stackPanelQuantity.Children.Add(labelQuantity);
            stackPanelQuantity.Children.Add(textBoxQuantity);
            horizontalStackPanel.Children.Add(stackPanelPrice);
            horizontalStackPanel.Children.Add(stackPanelQuantity);
            horizontalStackPanel.Children.Add(button);

            WholesalePrices.Children.Add(horizontalStackPanel);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            string buttonName = (sender as Button).Name;
            foreach (StackPanel s in WholesalePrices.Children) {
                if (s.Name.Length == buttonName.Length) {
                    WholesalePrices.Children.Remove(s);
                    break;
                }
            }
        }

        private void MakeRed(Control element, out bool add) {
            element.Background = Brushes.Red;
            add = false;
        }
    }
}
