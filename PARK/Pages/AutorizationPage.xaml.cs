using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;

using System.Windows.Shapes;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        public MainWindow mainWindow;
        public AutorizationPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            polelogin.Focus();
        }


        private async void btnlog_Click(object sender, RoutedEventArgs e)
        {
            string login = polelogin.Text;
            string password = polepassword.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            // Создаем объект для хранения данных аутентификации
            var credentials = new { login = login, password = password };

            // Сериализуем объект в JSON
            string json = JsonConvert.SerializeObject(credentials);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://api-all.ru/api/login",
                        new StringContent(json, Encoding.UTF8, "application/json"));

                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    string token = responseObject.token;

                    // Сохраняем токен в настройках приложения
                    Token.token = token;
                    Properties.Settings.Default.Token = token;
                    Properties.Settings.Default.Save();
                   
                    // Переход на другую страницу или какая-то другая логика после успешной аутентификации
                    FrameManager.MainFrame.Navigate(new MainPage(mainWindow));
                }
               
                catch (Exception)
                {
                    // Ошибка при неправильно введенных данных
                    MessageBox.Show("Проверьте правильность введенных данных");
                }
            }
        }

    }
}
