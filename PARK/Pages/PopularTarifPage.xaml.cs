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
        }
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private async Task<string> GetPopularTarif(string accessToken)
        {
            string name = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/rate");
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    string jsonString = await response.Content.ReadAsStringAsync();
                    var tarif = JsonConvert.DeserializeObject<Tarif>(jsonString);
                    name = tarif.Name;

                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
            }
            return name;
        }
        private async void LoadData()
        {
            // Получаем данные из API

            string name = await GetPopularTarif(Token.token);

            // Присваиваем данные источником данных для DataGrid
            nameTarif.Text = name;

        }
    }
}
