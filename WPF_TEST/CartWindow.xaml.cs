using Word = Microsoft.Office.Interop.Word;
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
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        public List<Tuple<Drug, int, string>> Basket;
        private string currentButtonName = "a";
        private string Login = "";
        public CartWindow()
        {
            InitializeComponent();
        }
        public CartWindow(List<Tuple<Drug, int, string>> basket, string login) : this() {
            Basket = basket;
            Login = login;
            if (Basket.Count != 0)
            {
                double resultSum = 0;
                foreach (Tuple<Drug, int, string> input in Basket)
                {
                    Drug drug = input.Item1;
                    int boughtAmount = input.Item2;
                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(drug.URL_photo, UriKind.Absolute);
                    bitmap.EndInit();
                    Image image = new Image
                    {
                        Source = bitmap,
                        Height = 150,
                        Width = 150
                    };
                    TextBlock info = new TextBlock
                    {
                        Text = $"{drug.Drug_name}, {drug.Weight_Volume}\nЦена:{input.Item3}",
                        FontSize = 18,
                        Width = 250,
                        TextWrapping = TextWrapping.Wrap
                    };
                    TextBlock amount = new TextBlock
                    {
                        Text = $"Количество\n{input.Item2}",
                        FontSize = 18,
                        Width = 100,
                        TextWrapping = TextWrapping.Wrap
                    };
                    TextBlock sum = new TextBlock
                    {
                        FontSize = 18,
                        Width = 100,
                        TextWrapping = TextWrapping.Wrap
                    };
                    double s = Convert.ToDouble(input.Item3.Replace('.', ','));
                    resultSum += s*input.Item2;
                    sum.Text = $"Сумма \n{s*input.Item2} грн.";
                    Button deleteButton = new Button();
                    deleteButton.Click += Button_Click1;
                    deleteButton.Name = currentButtonName;
                    Image image1 = new Image();
                    BitmapImage bitmap1 = new BitmapImage();
                    bitmap1.BeginInit();
                    bitmap1.UriSource = new Uri("C:\\Users\\Андрей\\Documents\\Visual Studio 2017\\Projects\\WPF_TEST\\WPF_TEST\\Icons\\minus.png", UriKind.Absolute);
                    bitmap1.EndInit();
                    image1.Source = bitmap1;
                    deleteButton.Content = image1;
                    stackPanel.Name = currentButtonName.Replace('a', 'b');
                    currentButtonName += "a";
                    stackPanel.Children.Add(image);
                    stackPanel.Children.Add(info);
                    stackPanel.Children.Add(amount);
                    stackPanel.Children.Add(sum);
                    stackPanel.Children.Add(deleteButton);
                    content.Children.Add(stackPanel);
                    AddPurchase.Visibility = Visibility.Visible;
                }
                Result.Content = $"Итого: {resultSum} грн.";
            }
            else {
                TextBlock textBlock = new TextBlock
                {
                    FontSize = 25,
                    Text = "Ваша корзина пуста"
                };
                content.Children.Add(textBlock);
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            string buttonName = (sender as Button).Name;
            for (int i = 0; i < content.Children.Count; i++) { 
                if ((content.Children[i] as StackPanel).Name.Length == buttonName.Length)
                {
                    content.Children.RemoveAt(i);
                    Basket.RemoveAt(i);
                    double result = 0;
                    foreach (Tuple<Drug, int, string> combo in Basket) {
                        double s = Convert.ToDouble(combo.Item3.Replace('.', ','));
                        result += s * combo.Item2;
                    }

                    if (result == 0)
                    {
                        TextBlock textBlock = new TextBlock
                        {
                            FontSize = 25,
                            Text = "Ваша корзина пуста"
                        };
                        content.Children.Add(textBlock);
                        Result.Content = "";
                        AddPurchase.Visibility = Visibility.Collapsed;
                    }
                    else {
                        Result.Content = $"Итого: {result} грн.";
                    }
                    break;
                }
            }

        }

        private void AddPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (Login == String.Empty) {
                MessageBox.Show("Для совершения покупок необходима регистрация");
            }
            else
            {
                MessageBox.Show($"Спасибо за покупку, {Login}");
                string chequeID = Drug.AddPurchase(Basket, Login);
                Word.Application app = new Word.Application();
                Word.Document doc = app.Documents.Add(Visible: true);
                Word.Range r = doc.Range();
                Word.Table t = doc.Tables.Add(r, Basket.Count + 1, 4);
                
                foreach (Word.Row row in t.Rows)
                {
                    foreach (Word.Cell cell in row.Cells)
                    {
                        if (cell.RowIndex == 1)
                        {
                            switch (cell.ColumnIndex) {
                                case 1:
                                    cell.Range.Text = "Название, вес";
                                    break;
                                case 2:
                                    cell.Range.Text = "Цена";
                                    break;
                                case 3:
                                    cell.Range.Text = "Количество";
                                    break;
                                case 4:
                                    cell.Range.Text = "Сумма";
                                    break;
                            }
                            
                            cell.Range.Bold = 1;
                            cell.Range.Font.Name = "verdana";
                            cell.Range.Font.Size = 10;

                            cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        else
                        {
                            switch (cell.ColumnIndex) {
                                case 1:
                                    cell.Range.Text = $"{Basket[cell.RowIndex-2].Item1.Drug_name}, " +
                                        $"{Basket[cell.RowIndex - 2].Item1.Weight_Volume}";
                                    break;
                                case 2:
                                    cell.Range.Text = $"{Basket[cell.RowIndex - 2].Item3} грн.";
                                    break;
                                case 3:
                                    cell.Range.Text = $"{Basket[cell.RowIndex - 2].Item2} грн.";
                                    break;
                                case 4:
                                    cell.Range.Text = $"{Convert.ToDouble(Basket[cell.RowIndex - 2].Item3) * Basket[cell.RowIndex - 2].Item2}";
                                    break;
                            }
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
                }
                Basket.RemoveRange(0, Basket.Count);
            }
            Close();
        }
    }
}
