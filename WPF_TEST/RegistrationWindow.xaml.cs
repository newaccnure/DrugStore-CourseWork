using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace WPF_TEST
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        MenuItem Authorization { set; get; }
        MenuItem Registration { set; get; }
        public RegistrationWindow(MenuItem authorization, MenuItem registration)
        {
            InitializeComponent();
            Authorization = authorization;
            Registration = registration;
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (password1.Password != password2.Password)
            {
                passwordPopup.IsOpen = true;
            }
            else {
                if (Logic.CheckLogin(Login.Text))
                {
                    MessageBox.Show("Успешная регистрация");
                    using (MySqlConnection connection = new MySqlConnection(Logic.connectionString)) {
                        connection.Open();
                        MySqlCommand sqlCommand = new MySqlCommand($"CALL InsertUser('{Login.Text}','{Email.Text}','{password1.Password}')",connection);
                        sqlCommand.ExecuteNonQuery();
                    }
                    Registration.Header = "Добро пожаловать, " + Login.Text;
                    Authorization.Visibility = Visibility.Collapsed;
                    Close();
                }
                else {
                    MessageBox.Show("Такой логин уже существует");
                }
            }
        }
        
    }
}
