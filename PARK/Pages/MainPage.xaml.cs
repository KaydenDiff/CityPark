using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
       
        public MainWindow mainWindow;
        public string fullName { get; set; }
        public MainPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadFullName();
        }

        private async Task<string> GetFullNameFromAPI(string accessToken)
        {
            string fullName = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/full");
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    fullName = await response.Content.ReadAsStringAsync();
                  
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
            }
            return fullName;
        }
        private async Task<string> GetRoleFromAPI(string accessToken)
        {
            string RoleName = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/getRole");
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    RoleName = await response.Content.ReadAsStringAsync();

                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
            }
            return RoleName;
        }
        private async Task Logout(string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/logout");
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    // Обработка успешного выхода из системы
                    MessageBox.Show("Вы успешно вышли из системы.");

                    // Очистка токена доступа (если необходимо)
                    accessToken = string.Empty;
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
            }
        }
            private async void LoadFullName()
        {
            string fullName = await GetFullNameFromAPI(Token.token);
            string roleName = await GetRoleFromAPI(Token.token);

            // Присваиваем полученное ФИО и роль свойству Text элементов TextBlock
            FN.Text = fullName;
            Role.Text = roleName;
            if (Role.Text == "admin")
            {
                // Показываем кнопки для администратора
                order.Visibility = Visibility.Visible;
                tarif.Visibility = Visibility.Visible;
                orders.Visibility = Visibility.Visible;
                profit.Visibility = Visibility.Visible;
                souvenirs.Visibility = Visibility.Visible;
                Role.Text = "Администратор";
            }
            else if (Role.Text == "manager")
            {
                // Показываем кнопки для менеджера
                order.Visibility = Visibility.Visible;
                souvenirs.Visibility = Visibility.Visible;
                Role.Text = "Менеджер";

            }
            else if (Role.Text == "editor")
            {
                // Показываем кнопки для менеджера
                order.Visibility = Visibility.Visible;
                souvenirs.Visibility = Visibility.Visible;
                Role.Text = "Редактор";

            }
            else
            {
                // Если роль не определена или неизвестна, скрываем все кнопки
                order.Visibility = Visibility.Collapsed;
                tarif.Visibility = Visibility.Collapsed;
                orders.Visibility = Visibility.Collapsed;
                profit.Visibility = Visibility.Collapsed;
                Role.Text = "Пользователь системы";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new OrderListPage(mainWindow));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new ProfitPage(mainWindow));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new PopularTarifPage(mainWindow));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new EmployeeListPage(mainWindow));
        }

     
        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
          
            try
            {
                await Logout(Token.token);
                // Дополнительные действия после успешного выхода
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе из системы: {ex.Message}");
            }
            FrameManager.MainFrame.Navigate(new AutorizationPage(mainWindow));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new SouvenirsPage(mainWindow));
        }
    }
}
