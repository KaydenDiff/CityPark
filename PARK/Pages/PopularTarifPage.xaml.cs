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
using System.Xml.Linq;
using Newtonsoft.Json;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для PopularTarifPage.xaml
    /// </summary>
    public partial class PopularTarifPage : Page
    {
        public MainWindow mainWindow;
        public PopularTarifPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            LoadData();
        }
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private async Task<List<string>> GetPopularTarif(string accessToken)
        {
            List<string> tarifInfo = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/popular");
                    response.EnsureSuccessStatusCode(); // Ensure success status code

                    string jsonString = await response.Content.ReadAsStringAsync();
                    tarifInfo = JsonConvert.DeserializeObject<List<string>>(jsonString);
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Error while requesting API: {ex.Message}");
                }
            }
            return tarifInfo;
        }
        private async void LoadData()
        {
            // Получаем данные из API
            List<string> tarifInfo = await GetPopularTarif(Token.token);

            // Очищаем предыдущий контент
            nameTarif.Text = string.Empty;

            // Выводим каждую строку информации о тарифе
            foreach (string info in tarifInfo)
            {
                nameTarif.Text += info + "\n"; // Предполагая, что nameTarif - это TextBlock или TextBox
            }

        }
    }
}