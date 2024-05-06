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
using Newtonsoft.Json;
using System.Data;


namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeListPage.xaml
    /// </summary>
    public partial class EmployeeListPage : Page
    {
        public MainWindow mainWindow;
        public EmployeeListPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadData();
        }
        private async Task<List<UsersList>> GetDataFromApi(string accessToken)
        {
            List<UsersList> data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    // Запрос на получение списка пользователей
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/users");
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        string usersJson = await response.Content.ReadAsStringAsync();
                        List<UsersList> users = JsonConvert.DeserializeObject<List<UsersList>>(usersJson);

                        // Запрос на получение списка ролей
                        HttpResponseMessage roleResponse = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/roles");
                        roleResponse.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                        string rolesJson = await roleResponse.Content.ReadAsStringAsync();
                        List<RoleList> rolesData = JsonConvert.DeserializeObject<List<RoleList>>(rolesJson);

                        // Сопоставление идентификаторов ролей с их названиями
                        var rolesDictionary = rolesData.ToDictionary(role => role.Id, role => role.Name);

                        // Выбор нужных свойств из объектов User и замена идентификаторов ролей на их названия
                        data = users.Where(u => rolesDictionary.ContainsKey(u.Role_id) && rolesDictionary[u.Role_id] != "user")
                    .Select(u => new UsersList
                    {
                        fullName = $"{u.surname} {u.name} {u.patronymic}",
                        roleName = rolesDictionary.ContainsKey(u.Role_id) ?
(rolesDictionary[u.Role_id] == "admin" ? "Администратор" :
(rolesDictionary[u.Role_id] == "manager" ? "Менеджер" :
(rolesDictionary[u.Role_id] == "editor" ? "Редактор" :
rolesDictionary[u.Role_id]))) : "Unknown"
                }).ToList();
                    }

                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"API request failed with status code {response.StatusCode}: {responseContent}");
                    }
                }
                catch (JsonReaderException ex)
                {
                    MessageBox.Show($"Ошибка при парсинге JSON: {ex.Message}");
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
                }
            }

            return data;
        }
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tokenProvider = new TokenProvider();
            FrameManager.MainFrame.Navigate(new AddEmployeePage(mainWindow));
        }
        private async void LoadData()
        {
            // Получаем данные из API
          
            List<UsersList> userData = await GetDataFromApi(Token.token);

            // Присваиваем данные источником данных для DataGrid
            ListVK.ItemsSource = userData;

        }

        
    }
}

