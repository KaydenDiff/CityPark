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
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        public MainWindow mainWindow;
        public AddEmployeePage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadData();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private void AddEmployeeButton_click(object sender, RoutedEventArgs e)
        {
            RegisterUser();
            MessageBox.Show("Сотрудник создан успешно!");
        }

        private async void LoadData()
        {
            try
            {
                // Получаем роли из API и заполняем ComboBox
                await FillRoleComboBox(Token.token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private async Task FillRoleComboBox(string accessToken)
        {
            try
            {
                // Получаем роли из API
                string rolesJson = await GetRoleFromAPI(accessToken);

                // Десериализуем JSON в список объектов Role
                List<RoleList> roles = JsonConvert.DeserializeObject<List<RoleList>>(rolesJson);

                // Очищаем ComboBox перед добавлением новых элементов
                comboBoxRoles.Items.Clear();

                // Добавляем роли в ComboBox
                foreach (var role in roles)
                {
                    comboBoxRoles.Items.Add(role.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении списка ролей: {ex.Message}");
            }
        }

        private async Task<string> GetRoleFromAPI(string accessToken)
        {
            string RoleName = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/roles");
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
        private async void RegisterUser()
        {
            try
            {
                // Получаем выбранную роль из ComboBox
                string selectedRole = comboBoxRoles.SelectedItem.ToString();

                // Создаем объект пользователя с данными из текстовых полей и выбранной ролью
                User user = new User
                {
                    Login = logintext.Text,
                    Password = passtext.Password,
                    Name = nametext.Text,
                    Surname = surtext.Text,
                    Patronymic = pattext.Text,
                    Role = selectedRole // Устанавливаем выбранную роль
                };

                // Преобразуем объект пользователя в формат JSON
                string userJson = JsonConvert.SerializeObject(user);

                // Создаем HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/reg", new StringContent(userJson, Encoding.UTF8, "application/json"));

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Пользователь успешно зарегистрирован!");
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при регистрации пользователя: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации пользователя: {ex.Message}");
            }
        }
    }
}