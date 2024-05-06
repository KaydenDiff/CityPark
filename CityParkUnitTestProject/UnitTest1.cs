using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using PARK.Pages;
using System.Net.Http;
using Moq;
using PARK;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Net;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace CityParkUnitTestProject
{
    [TestClass]
    public class AuthorizationPageTests
    {
        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null);
            // Act
            bool isAuthenticated = await authorizationPage.AuthenticateAsync("sasa-ouy", "Ss1@");
            Assert.IsTrue(isAuthenticated);
        }

        [TestMethod]
        public async Task AuthenticateAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null);

            // Act
            bool isAuthenticated = await authorizationPage.AuthenticateAsync("invalidLogin", "invalidPassword");

            // Assert
            Assert.IsFalse(isAuthenticated);
        }
        [TestMethod]
        public async Task AuthenticateAsync_EmptyCredentials_ReturnsFalse()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null);

            // Act
            bool isAuthenticated = await authorizationPage.AuthenticateAsync("", "");

            // Assert
            Assert.IsFalse(isAuthenticated);
        }
        [TestMethod]
        public async Task AuthenticateAsync_EmptyLogin_ReturnsFalse()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null);

            // Act
            bool isAuthenticated = await authorizationPage.AuthenticateAsync("", "asdasdad");

            // Assert
            Assert.IsFalse(isAuthenticated);
        }
        [TestMethod]
        public async Task AuthenticateAsync_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null);

            // Act
            bool isAuthenticated = await authorizationPage.AuthenticateAsync("asdasd", "");

            // Assert
            Assert.IsFalse(isAuthenticated);
        }


    }
    [TestClass]
    public class AddEmployeeTest
    {
        [TestMethod]
        public async Task RegisterUser_Successful_ReturnsTrue()
        {
            string password = "Ss1@";
            string login = "sasa-ouy";
            bool actual = false;
            bool expected = true;

            // Авторизация и получение токена
            var credentials = new { login = login, password = password };
            string token = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/login",
                        new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    token = JsonConvert.DeserializeObject<string>(responseBody);

                    // Сохраняем токен в настройках приложения
                    Token.token = token;
                }
                catch (Exception)
                {
                    // Ошибка при неправильно введенных данных
                    MessageBox.Show("Ошибка аутентификации");
                }
            }

            // Вызов метода RegisterUser, передавая токен
            User user = new User
            {
                login = "validLogin",
                password = "validPassword1+",
                name = "John",
                surname = "Doe",
                patronymic = "Smith",
                phone = "123456789",
                role_id = 2
            };

            string userJson = JsonConvert.SerializeObject(user);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // Установка заголовка авторизации
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/reg", new StringContent(userJson, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сотрудник успешно добавлен!");
                        actual = true;
                    }
                    else
                    {
                        MessageBox.Show($"{response}");
                        actual = false;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                    actual = false;
                }
            }

            Assert.AreEqual(expected, actual);
        }
    }
    [TestClass]

    public class IncomeTests
    {
        [TestMethod]
        public async Task ProfitPageTests()
        {
            string password = "Ss1@";
            string login = "sasa-ouy";
            string expected = "7040";

            string responseData = string.Empty;

            var startDate = new DateTime(2024, 04, 29, 10, 21, 09);
            var endDate = new DateTime(2024, 05, 04, 11, 12, 31);
            // Авторизация и получение токена
            var credentials = new { login = login, password = password };
            string token = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/login",
                        new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    token = JsonConvert.DeserializeObject<string>(responseBody);// Предполагается, что токен доступен здесь

                    // Сохраняем токен в настройках приложения
                    Token.token = token;
                }
                catch (Exception)
                {
                    // Ошибка при неправильно введенных данных
                    MessageBox.Show("Ошибка аутентификации");
                }
            }

            try
            {


                using (HttpClient client = new HttpClient())
                {
                    // Ваш токен здесь


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
            Assert.AreEqual(expected, responseData);
        }

    }

    [TestClass]
    public class AddSouvenirTests
    {
        [TestMethod]
        public async Task AddSouvenir_Successful_ReturnsTrue()
        {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";
            bool actual = false;
            bool expected = true;

            // Авторизация и получение токена
            var credentials = new { login = login, password = password };
            string token = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/login",
                        new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    token = JsonConvert.DeserializeObject<string>(responseBody);// Предполагается, что токен доступен здесь

                    // Сохраняем токен в настройках приложения
                    Token.token = token;
                }
                catch (Exception)
                {
                    // Ошибка при неправильно введенных данных
                    MessageBox.Show("Ошибка аутентификации");
                }
            }

            try
            {
                // Создание объекта данных для сувенира
                var souvenirData = new
                {
                    name = "Sample Souvenir",
                    price = 14.00,
                    description = "Sample Description",
                    category_souvenir_id = 1
                };

                // Сериализация объекта данных в формат JSON
                string jsonData = JsonConvert.SerializeObject(souvenirData);

                // Создание HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    // Добавление токена в заголовок запроса
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Добавление типа содержимого в заголовок
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Отправка запроса на сервер
                    HttpResponseMessage response = await client.PostAsync("http://ladyaev-na.tepk-it.ru/api/addSouvenir",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сувенир успешно добавлен!");
                        actual = true;
                    }
                    else
                    {
                        actual = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Assert.AreEqual(expected, actual);
        }
    }

}