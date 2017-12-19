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
    /// Логика взаимодействия для EditSymptomWindow.xaml
    /// </summary>
    public partial class EditSymptomWindow : Window
    {
        private int SymptomID;
        private DataGrid DataGrid;
        public EditSymptomWindow(DataGrid dataGrid)
        {
            InitializeComponent();
            DataGrid = dataGrid;
        }

        public EditSymptomWindow(int symptomID, DataGrid dataGrid) : this(dataGrid) {
            SymptomID = symptomID;
            AddButton.Content = "Изменить";
            SymptomTextBox.Text = Symptom.GetSymptomById(symptomID);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddButton.Content.ToString() == "Добавить")
            {
                //Add
                if (SymptomTextBox.Text != String.Empty)
                {
                    string addSymptomQuery = $"CALL InsertSymptom('{SymptomTextBox.Text}')";
                    Logic.InsertInformation(addSymptomQuery);
                    Logic.ShowTable(DataGrid, "CALL GetSymptoms()");
                    Close();
                }
                else
                {
                    MessageBox.Show("Введите название симптома");
                }
            }
            else {
                //Change
                Symptom.UpdateSymptom(SymptomID, SymptomTextBox.Text);
                Logic.ShowTable(DataGrid, "CALL GetSymptoms()");
                Close();
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
