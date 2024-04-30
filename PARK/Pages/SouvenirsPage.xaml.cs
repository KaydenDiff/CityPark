using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для SouvenirsPage.xaml
    /// </summary>
    public partial class SouvenirsPage : Page
    {
        public MainWindow mainWindow;
        public SouvenirsPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadData();
        }
        private async Task<List<Souvenir>> GetSouvenirsFromApi(string accessToken)
        {
            List<Souvenir> data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/souvenirs");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<List<Souvenir>>(responseBody);

                    // Загрузка изображений сувениров
                    foreach (var souvenir in data)
                    {
                      
                        souvenir.Photo = "http://ladyaev-na.tepk-it.ru/storage/" + souvenir.Photo;// Предположим, что путь к фото хранится без базового URL
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке сувениров: {ex.Message}");
                }
            }
            return data;
        }

        private async void LoadData()
        {
            try
            {
                // Получаем данные из API
                List<Souvenir> souvenirs = await GetSouvenirsFromApi(Token.token);

                // Присваиваем данные источником данных для ListBox
                souvenirListBox.ItemsSource = souvenirs;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сувениров: {ex.Message}");
            }
            souvenirListBox.Visibility = Visibility.Visible;
        }

       
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private void EditSouvenir_Click(object sender, RoutedEventArgs e)
        {
            // Получаем объект Souvenir, связанный с кнопкой "Изменить"
            Button button = (Button)sender;
            Souvenir souvenir = (Souvenir)button.DataContext;

            // Переходим на страницу редактирования сувенира, передавая объект Souvenir
            FrameManager.MainFrame.Navigate(new EditSouvenirPage(souvenir, mainWindow));
        }
        private async Task DeleteSouvenir(int souvenirId, string accessToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Устанавливаем заголовок авторизации
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // URL для удаления сувенира с определенным идентификатором
                    string url = $"http://ladyaev-na.tepk-it.ru/api/deleteSouvenir/{souvenirId}";

                    // Отправляем DELETE запрос для удаления сувенира
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    // Проверяем успешность запроса
                    if (response.StatusCode == HttpStatusCode.Gone)
                    {
                        MessageBox.Show("Сувенир успешно удален!");
                        // Дополнительные действия по обновлению пользовательского интерфейса или переходу на другую страницу
                        FrameManager.MainFrame.Navigate(new MainPage(mainWindow));
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        dynamic errorJson = JsonConvert.DeserializeObject(errorResponse);
                        string errorMessage = errorJson.message;
                        MessageBox.Show($"Ошибка при удалении сувенира: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}");
            }
        }
        private async void DeleteSouvenir_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Souvenir souvenir = (Souvenir)button.DataContext;
            try
            {
                // Получаем идентификатор сувенира для удаления
                int souvenirId =  souvenir.Id;

                // Получаем токен доступа
                string accessToken = Token.token;

                // Отправляем запрос на удаление сувенира
                await DeleteSouvenir(souvenirId, accessToken);

                // После успешного удаления, выполните необходимые действия, например, обновите интерфейс или перейдите на другую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении сувенира: {ex.Message}");
            }
        }
    }
}
