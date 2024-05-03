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
using System.Net.NetworkInformation;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderListPage.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        public MainWindow mainWindow;
        public OrderListPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadCartItems();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private async Task<List<Cart>> GetCartItemsAsync(string accessToken)
        {
            List<Cart> cartItems = new List<Cart>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/showCart");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        cartItems = JsonConvert.DeserializeObject<List<Cart>>(jsonResponse);

                        // Для каждого элемента корзины получаем данные о пользователе и сувенире
                        foreach (var item in cartItems)
                        {
                            // Получаем данные о пользователе
                            HttpResponseMessage userResponse = await client.GetAsync($"http://ladyaev-na.tepk-it.ru/api/user/show/{item.user_id}");
                            if (userResponse.IsSuccessStatusCode)
                            {
                                string userJsonResponse = await userResponse.Content.ReadAsStringAsync();
                                User user = JsonConvert.DeserializeObject<User>(userJsonResponse);
                                // Устанавливаем полное имя пользователя в свойство FullName
                                item.FullName = $"{user.surname} {user.name} {user.patronymic}";
                                item.phone = user.phone;
                            }
                            else
                            {
                                // Обработка неуспешного ответа
                            }

                            // Получаем данные о сувенире
                            HttpResponseMessage souvenirResponse = await client.GetAsync($"http://ladyaev-na.tepk-it.ru/api/souvenir/{item.souvenir_id}");
                            if (souvenirResponse.IsSuccessStatusCode)
                            {
                                string souvenirJsonResponse = await souvenirResponse.Content.ReadAsStringAsync();
                                Souvenir souvenir = JsonConvert.DeserializeObject<Souvenir>(souvenirJsonResponse);
                                // Устанавливаем название сувенира в свойство SouvenirName
                                item.SouvenirName = souvenir.name;
                            }
                            else
                            {
                                // Обработка неуспешного ответа
                            }
                        }
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"API request failed with status code {response.StatusCode}: {responseContent}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}");
                }

                return cartItems;
            }
        }
        private async void LoadCartItems()
        {
            // Получаем данные из API
            List<Cart> cartItems = await GetCartItemsAsync(Token.token);

            // Присваиваем данные источником данных для DataGrid
            ListVK.ItemsSource = cartItems;
        }

    }
}

