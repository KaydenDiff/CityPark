using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfitPage.xaml
    /// </summary>
    public partial class ProfitPage : Page
    {
        public MainWindow mainWindow;
        public ProfitPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
        }
        public class IncomeData
        {
            public double Income { get; set; }
        }
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private async Task<string> GetIncomeForPeriod(DateTime startDate, DateTime endDate)
        {
            string responseData = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Ваш токен здесь
                    string token = Token.token;

                    // Добавляем заголовок авторизации
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var parameters = new Dictionary<string, string>
            {
                { "created_at", startDate.ToString("yyyy-MM-dd HH:mm:ss") },
                { "updated_at", endDate.ToString("yyyy-MM-dd HH:mm:ss") }
            };

                    string queryString = string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
                    string url = $"http://ladyaev-na.tepk-it.ru/api/income?{queryString}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        // Обработка успешного ответа
                        responseData = await response.Content.ReadAsStringAsync();

                        // Декодирование Unicode-строки в обычную строку
                        responseData = Regex.Unescape(responseData);
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при получении дохода: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}");
            }
            return responseData;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime startDate = startDatePicker.Value ?? DateTime.MinValue;
                DateTime endDate = endDatePicker.Value ?? DateTime.MaxValue;

                string responseData = await GetIncomeForPeriod(startDate, endDate);
                profit.Text = responseData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении дохода: {ex.Message}");
            }
        }
    }
}
