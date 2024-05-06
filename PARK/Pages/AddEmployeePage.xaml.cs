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


        public string LoginText
        {
            get { return logintext.Text; }
            set { logintext.Text = value; }
        }
        public string Password
        {
            get { return passtext.Password; }
            set { passtext.Password = value; }
        }
        public string NameText
        {
            get { return nametext.Text; }
            set { nametext.Text = value; }
        }
        public string SurnameText
        {
            get { return surtext.Text; }
            set { surtext.Text = value; }
        }

        // Свойство для доступа к отчеству
        public string PatronymicText
        {
            get { return pattext.Text; }
            set { pattext.Text = value; }
        }

        // Свойство для доступа к номеру телефона
        public string PhoneText
        {
            get { return telephone.Text; }
            set { telephone.Text = value; }
        }
        public int SelectedRoleId
        {
            get
            {
                if (comboBoxRoles.SelectedItem == null)
                {

                    return 2;
                }
                else
                {
                    // Возвращаем идентификатор выбранной роли из словаря по тексту
                    string selectedRoleName = comboBoxRoles.SelectedItem.ToString();
                    return roleDictionary[selectedRoleName];
                }
            }
            set
            {
                foreach (var pair in roleDictionary)
                {
                    if (pair.Value == value)
                    {
                        comboBoxRoles.SelectedItem = pair.Key;
                        break;
                    }
                }
            }
        }

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
            await RegisterUser(Token.token);
        }

        private async void LoadData()
        {
            try
            { // Получаем роли из API и заполняем ComboBox
                await FillRoleComboBox(Token.token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private Dictionary<string, int> roleDictionary = new Dictionary<string, int>();
        private Dictionary<string, string> roleTranslations = new Dictionary<string, string>
{
    { "admin", "Администратор" },
    { "manager", "Менеджер" },
    { "editor", "Редактор" },
};

        private async Task FillRoleComboBox(string accessToken)
        {
            try
            {
                string rolesJson = await GetRoleFromAPI(accessToken);
                List<RoleList> roles = JsonConvert.DeserializeObject<List<RoleList>>(rolesJson);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Очистка ComboBoxRoles перед добавлением новых элементов
                    comboBoxRoles.Items.Clear();
                    roleDictionary.Clear(); // Очистка словаря

                    // Добавление ролей в ComboBoxRoles и их идентификаторов в словарь
                    foreach (var role in roles)
                    {
                        if (role.Name != "user")
                        {
                            // Получение русского названия роли из словаря
                            string russianRoleName = roleTranslations.ContainsKey(role.Name) ? roleTranslations[role.Name] : role.Name;
                            comboBoxRoles.Items.Add(russianRoleName);
                            roleDictionary.Add(russianRoleName, role.Id); // Сохранение идентификатора роли в словаре
                        }
                    }
                });
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
        public async Task<bool> RegisterUser(string accessToken)
        {
            if (string.IsNullOrEmpty(logintext.Text) || string.IsNullOrEmpty(passtext.Password) ||
                string.IsNullOrEmpty(nametext.Text) || string.IsNullOrEmpty(surtext.Text))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                });
                return false;
            }

            try
            {
                int roleId = roleDictionary[comboBoxRoles.SelectedItem.ToString()];
                User user = new User
                {
                    login = logintext.Text,
                    password = passtext.Password,
                    name = nametext.Text,
                    surname = surtext.Text,
                    patronymic = pattext.Text,
                    phone = telephone.Text,
                    role_id = roleId
                };

                string userJson = JsonConvert.SerializeObject(user);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Установка заголовка авторизации
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/reg", new StringContent(userJson, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Сотрудник успешно добавлен!");
                        });
                        return true; // Возвращаем true в случае успешного добавления пользователя
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Ошибка при добавлении сотрудника: {errorMessage}");
                        });
                        return false; // Возвращаем false, если произошла ошибка при добавлении пользователя
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Ошибка при регистрации сотрудника: {ex.Message}");
                });
                return false; // Возвращаем false в случае возникновения исключения
            }
        }
    }
}