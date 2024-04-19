using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        public MainWindow mainWindow;
        public AutorizationPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            polelogin.Focus();
        }
    
 
        private async void btnlog_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://your-api-url.com/endpoint");
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Здесь можно обработать полученные данные из API, например, вывести их в TextBox или ListView
                    // Например:
                    resultTextBox.Text = responseBody;
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                }
            }
        }
    }
}
