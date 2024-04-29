using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
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
using System.Diagnostics;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditSouvenirPage.xaml
    /// </summary>
    public partial class EditSouvenirPage : Page
    {
        private Souvenir souvenirInstance { get; set; }
        public MainWindow mainWindow;
        private int souvenir_id { get; set; }
        public EditSouvenirPage(Souvenir souvenir, MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            this.souvenir_id = souvenir.Id; // Неправильно, должно быть this.souvenirId = souvenir.Id;
            DataContext = souvenir;
            LoadData(souvenir);
        }
       
        private async Task UpdateSouvenir(int souvenirId, string updatedName, string updatedDescription, double updatedPrice, string accessToken)
        {
            try
            {
                // Загружаем существующий сувенир, чтобы сравнить с обновленными данными
                Souvenir existingSouvenir = await GetSouvenir(souvenirId, accessToken);

                // Создаем объект JSON только с измененными данными сувенира
                var updatedData = new
                {
                    name = existingSouvenir != null && updatedName != existingSouvenir.Name ? updatedName : existingSouvenir.Name,
                    description = existingSouvenir != null && updatedDescription != existingSouvenir.Description ? updatedDescription : existingSouvenir.Description,
                    price = existingSouvenir != null && updatedPrice != existingSouvenir.Price ? updatedPrice : existingSouvenir.Price // используйте текущее значение, если оно не было изменено
                };


                // Преобразуем объект JSON в строку JSON
                string updatedDataJson = JsonConvert.SerializeObject(updatedData);

                // Создаем HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    // Устанавливаем заголовок авторизации
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // URL для редактирования сувенира с определенным идентификатором
                    string url = $"http://ladyaev-na.tepk-it.ru/api/updateSouvenir/{souvenirId}";

                    // Создаем контент для запроса с обновленными данными
                    HttpContent content = new StringContent(updatedDataJson, Encoding.UTF8, "application/json");

                    // Отправляем POST запрос для обновления сувенира
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    MessageBox.Show($"Souvenir ID:{updatedData}");
                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сувенир успешно обновлен!");
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при обновлении сувенира: {errorMessage}");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}, ");
            }

        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем обновленные данные сувенира из текстовых полей
                string updatedName = NameSouvenir.Text;
                string updatedDescription = DescriptionSouvenir.Text;
                double updatedPrice = Convert.ToDouble(PriceSouvenir.Text);


                // Получаем идентификатор сувенира из поля souvenirId
                int souvenirId = this.souvenir_id;

                // Получаем токен доступа
                string accessToken = Token.token;
                MessageBox.Show($"Souvenir ID: {souvenirId}, Name: {updatedName}, Description: {updatedDescription}, Price: {updatedPrice}");

                // Отправляем запрос на обновление сувенира
                await UpdateSouvenir(souvenirId, updatedName, updatedDescription, updatedPrice, accessToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении сувенира: {ex.Message}");
            }
        }
        private async Task<Souvenir> GetSouvenir(int souvenirId, string accessToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Устанавливаем заголовок авторизации
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // URL для получения сувенира по идентификатору
                    string url = $"http://ladyaev-na.tepk-it.ru/api/souvenir/{souvenirId}";

                    // Отправляем GET запрос для получения сувенира
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Читаем содержимое ответа
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Десериализуем JSON в объект Souvenir
                        Souvenir souvenir = JsonConvert.DeserializeObject<Souvenir>(responseContent);

                        return souvenir;
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Ошибка при получении сувенира: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}");
                return null;
            }
        }
        private async Task LoadData(Souvenir souvenir)
        {
            try
            {
                souvenirInstance = souvenir;
                NameSouvenir.Text = souvenirInstance.Name;
                DescriptionSouvenir.Text = souvenirInstance.Description;
                PriceSouvenir.Text = souvenirInstance.Price.ToString();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception loading data: {ex.Message}");
            }
        }
    }
}
