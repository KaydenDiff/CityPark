using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PARK.Pages
{
    public partial class AddsouvenirPage : Page
    {
        public MainWindow mainWindow;
        private string base64Image; // Переменная для хранения base64 представления изображения

        public AddsouvenirPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadData();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Добавьте await перед вызовом метода AddSouvenir
            await AddSouvenir(Token.token);
         
        }

        private async void LoadData()
        {
            try
            {
                // Добавьте await перед вызовом метода FillRoleComboBox
                await FillRoleComboBox(Token.token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private Dictionary<string, int> categoryDictionary = new Dictionary<string, int>();
        private Dictionary<string, string> categoryTranslations = new Dictionary<string, string>();

        private async Task FillRoleComboBox(string accessToken)
        {
            try
            {
                List<Category> categories = await GetCategoriesFromApi(accessToken);
                // Очистка ComboBox перед добавлением новых элементов
                comboBoxCategory.Items.Clear();
                categoryDictionary.Clear(); // Очистка словаря

                // Добавление категорий в ComboBox и их идентификаторов в словарь
                foreach (var category in categories)
                {
                    if (category.Name != "user")
                    {
                        // Получение русского названия категории из словаря
                        string russianCategoryName = categoryTranslations.ContainsKey(category.Name) ? categoryTranslations[category.Name] : category.Name;
                        comboBoxCategory.Items.Add(russianCategoryName);
                        categoryDictionary.Add(russianCategoryName, category.Id); // Сохранение идентификатора категории в словаре
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении списка категорий: {ex.Message}");
            }
        }

        private async Task AddSouvenir(string accessToken)
        {
            if (string.IsNullOrEmpty(NameSouvenir.Text) || string.IsNullOrEmpty(DescriptionSouvenir.Text) ||
                string.IsNullOrEmpty(PriceSouvenir.Text) || comboBoxCategory.SelectedItem == null || string.IsNullOrEmpty(base64Image))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля и добавьте изображение.");
                return;
            }
            try
            {
                // Получение выбранного идентификатора категории из словаря
                int categoryId = categoryDictionary[comboBoxCategory.SelectedItem.ToString()];

                // Создание объекта MultipartFormDataContent для добавления данных и файла
                using (var form = new MultipartFormDataContent())
                {
                    // Добавление текстовых данных
                    form.Add(new StringContent(NameSouvenir.Text), "name");
                    form.Add(new StringContent(DescriptionSouvenir.Text), "description");
                    form.Add(new StringContent(PriceSouvenir.Text), "price");
                    form.Add(new StringContent(categoryId.ToString()), "category_souvenir_id");

                    // Чтение файла в MemoryStream
                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                    using (var imageStream = new MemoryStream(imageBytes))
                    {
                        // Добавление файла в содержимое
                        form.Add(new StreamContent(imageStream), "photo", "souvenir_image.jpg");

                        // Создание HttpClient для отправки запроса
                        using (HttpClient client = new HttpClient())
                        {
                            // Добавление токена в заголовок запроса
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                            // Отправка запроса на сервер
                            HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/addSouvenir", form);

                            // Проверка успешности запроса
                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Сувенир успешно добавлен!");
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                MessageBox.Show($"Ошибка при добавлении сувенира: {errorMessage}");
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сувенира: {ex.Message}");
              
            }
            FrameManager.MainFrame.Navigate(new MainPage(mainWindow));
        }

        private async Task<List<Category>> GetCategoriesFromApi(string accessToken)
        {
            List<Category> categories = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/showCategorySouvenirs");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}");
                }
            }
            return categories;
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу изображения
                string selectedImagePath = openFileDialog.FileName;

                // Чтение байтов изображения из файла
                imageBytes = System.IO.File.ReadAllBytes(selectedImagePath);

                // Преобразование байтов изображения в строку Base64
                base64Image = Convert.ToBase64String(imageBytes);

                // Отображаем выбранное изображение в элементе imagePreview
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.EndInit();
                imagePreview.Source = bitmap;
            }
        }

    }
}
