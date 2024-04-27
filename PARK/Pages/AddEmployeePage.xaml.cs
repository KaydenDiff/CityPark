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
using System.Security.Policy;
using System.Xml.Linq;

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

        private async void AddEmployeeButton_click(object sender, RoutedEventArgs e)
        {   
         await RegisterUser();
          
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
        private Dictionary<string, int> roleDictionary = new Dictionary<string, int>();
        private async Task FillRoleComboBox(string accessToken)
        {
       
            try
            {
                string rolesJson = await GetRoleFromAPI(accessToken);
                List<RoleList> roles = JsonConvert.DeserializeObject<List<RoleList>>(rolesJson);

                // Очистка ComboBoxRoles перед добавлением новых элементов
                comboBoxRoles.Items.Clear();
                roleDictionary.Clear(); // Очистка словаря

                // Добавление ролей в ComboBoxRoles и их идентификаторов в словарь
                foreach (var role in roles)
                {
                    comboBoxRoles.Items.Add(role.Name);
                    roleDictionary.Add(role.Name, role.Id); // Сохранение идентификатора роли в словаре
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
        private async Task RegisterUser()
        {
            if (string.IsNullOrEmpty(logintext.Text) || string.IsNullOrEmpty(passtext.Password) ||
string.IsNullOrEmpty(nametext.Text) || string.IsNullOrEmpty(surtext.Text))

            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }
                try
            {
                // Получение выбранного идентификатора роли из словаря
                int roleId = roleDictionary[comboBoxRoles.SelectedItem.ToString()];
                // Создание объекта пользователя с данными из текстовых полей и выбранным идентификатором роли
                User user = new User
                {
                    login = logintext.Text,
                    password = passtext.Password,
                    name = nametext.Text,
                    surname = surtext.Text,
                    patronymic = pattext.Text,
                    role_id = roleId // Отправка идентификатора роли на сервер
                };

                // Преобразование объекта пользователя в JSON
                string userJson = JsonConvert.SerializeObject(user);

                // Создание HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/reg", new StringContent(userJson, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
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
            FrameManager.MainFrame.Navigate(new MainPage(mainWindow));
        }
    }
}