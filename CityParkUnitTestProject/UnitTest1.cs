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
using System.Security.Policy;

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
        public async Task AddEmployee_Successful_Returns422()
        {
            string password = "Ss1@";
            string login = "sasa-ouy";
            int actual = 0;
            int expected = 422;
            User user = null;

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
            user = new User
            {
                login = "",
                password = "validPasswor",
                name = "000",
                surname = "000",
                patronymic = "0000",
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
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var createdUser = JsonConvert.DeserializeObject<User>(responseContent);
                        string createdUserId = createdUser.id.ToString(); // Преобразование ID в строку             
                     


                        try
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            HttpResponseMessage deleteResponse = await client.DeleteAsync($"http://ladyaev-na.tepk-it.ru/api/user/delete/{createdUserId}");

                            if (deleteResponse.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Пользователь успешно удален!");
                            }
                            else
                            {
                                MessageBox.Show($"Ошибка при удалении пользователя: {deleteResponse.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
                        }
                    }
                    else if (response.StatusCode == (HttpStatusCode)422)
                    {
                        actual = (int)response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                 
                }
            }
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task AddEmployee_Successful_ReturnsTrue()
        {
            string password = "Ss1@";
            string login = "sasa-ouy";
            bool actual = false;
            bool expected = true;
            User user = null;

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
            user = new User
            {
                login = "validLogin1",
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
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var createdUser = JsonConvert.DeserializeObject<User>(responseContent);
                        string createdUserId = createdUser.id.ToString(); // Преобразование ID в строку             
                        actual = true;

                        try
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            HttpResponseMessage deleteResponse = await client.DeleteAsync($"http://ladyaev-na.tepk-it.ru/api/user/delete/{createdUserId}");

                            if (deleteResponse.StatusCode == HttpStatusCode.Gone)
                            {
                                MessageBox.Show("Пользователь успешно удален!");
                            }
                            else
                            {
                                MessageBox.Show($"Ошибка при удалении пользователя: {deleteResponse.StatusCode}");
                            }
                        }
                        catch (WebException ex)
                        {
                            if (ex.Response is HttpWebResponse webResponse && webResponse.StatusCode == HttpStatusCode.Gone)
                            {
                                MessageBox.Show("Ошибка 410: Ресурс удален. Пользователь не найден.");
                            }
                            else
                            {
                                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
                            }
                        }
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

        [TestMethod]
        public async Task ProfitPageTests_WithoutToken()
        {
            int actual = 0;
            int expected = 500;
            string responseData = string.Empty;

            var startDate = new DateTime(2024, 04, 29, 10, 21, 09);
            var endDate = new DateTime(2024, 04, 29, 10, 21, 09);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var parameters = new Dictionary<string, string>
            {
                { "created_at", startDate.ToString("yyyy-MM-dd HH:mm:ss") },
                { "updated_at", endDate.ToString("yyyy-MM-dd HH:mm:ss") }
            };

                    string queryString = string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
                    string url = $"http://ladyaev-na.tepk-it.ru/api/income?{queryString}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    actual = (int)response.StatusCode;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}");
               
            }

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task ProfitPageTests_TheSameData()
        {
            string password = "Ss1@";
            string login = "sasa-ouy";
            string expected = "0";

            string responseData = string.Empty;

            var startDate = new DateTime(2024, 04, 29, 10, 21, 09);
            var endDate = new DateTime(2024, 04, 29, 10, 21, 09);
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
        public async Task EditSouvenir_WithoutToken()
        {
            // Arrange
            int souvenirId = 13;
            bool actual = true;
            bool expected = false;
            try
            {
                // Создание объекта данных для сувенира
                var souvenirData = new
                {
                    name = "ADS",
                    price = 13.00,
                    description = "asda",
                };

                // Сериализация объекта данных в формат JSON
                string jsonData = JsonConvert.SerializeObject(souvenirData);

                // Создание HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    // Добавление токена в заголовок запроса


                    // Добавление типа содержимого в заголовок
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Отправка запроса на сервер
                    HttpResponseMessage response = await client.PostAsync($"http://ladyaev-na.tepk-it.ru/api/updateSouvenir/{souvenirId}",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сувенир успешно изменен!");
                        actual = true;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Ошибка 401: Не авторизован.");
                        actual = false;
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show("Ошибка 500: Внутренняя ошибка сервера.");
                        actual = false;
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при изменении сувенира. Код состояния: {response.StatusCode}");
                        actual = false;
                    }
                    Assert.AreEqual(expected, actual);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        [TestMethod]
        public async Task AddSouvenir_Successful_Returns422()
        {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";
            bool actual = true;
            bool expected = false;

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
                    name = "",
                    price = 0.00,
                    description = "",
                    category_souvenir_id = 0
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
      
        [TestMethod]
        public async Task AddSouvenir_WithoutToken()
        {
            // Arrange
            int actual = 0;
            int expected = 401;
          
            try
            {
                // Создание объекта данных для сувенира
                var souvenirData = new
                {
                    name = "ADS",
                    price = 13.00,
                    description = "asda",
                    category_souvenir_id = 0
                };

                // Сериализация объекта данных в формат JSON
                string jsonData = JsonConvert.SerializeObject(souvenirData);

                // Создание HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    // Добавление токена в заголовок запроса


                    // Добавление типа содержимого в заголовок
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Отправка запроса на сервер
                    HttpResponseMessage response = await client.PostAsync($"http://ladyaev-na.tepk-it.ru/api/addSouvenir",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    actual = (int)response.StatusCode;
                    Assert.AreEqual(expected, actual);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

    }
    [TestClass]
    public class PopularTarifTests
    {
        [TestMethod]
        public async Task PopularTarifTests_WithoutToken()
        {
            // Arrange
           
            int actual = 0;
            int expected = 500;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/popular");

                    actual = (int)response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
               
            }

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task PopularTarifTests_Successful_ReturnsTrue()
        {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";
            List<string> tarifInfo = null;
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
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                foreach (string info in tarifInfo)
                {
                    actual = true;
                }
            }
            catch (Exception)
            {
                // Ошибка при неправильно введенных данных
              
                actual = false;
            }
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class EditSouvenirTests
    {
        [TestMethod]
        public async Task EditSouvenir_Successful_ReturnsTrue()
        {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";
            int souvenirId = 13;
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
                    name = "Sample Souvenir13",
                    price = 13.00,
                    description = "Sample Description13",
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
                    HttpResponseMessage response = await client.PostAsync($"http://ladyaev-na.tepk-it.ru/api/updateSouvenir/{souvenirId}",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                       
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
        [TestMethod]
        public async Task EditSouvenir_Successful_Returns422()
        {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";
            int souvenirId = 13;
            int actual = 0;
            int expected = 422;

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
                    name = "",
                    price = 13.00,
                    description = "",
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
                    HttpResponseMessage response = await client.PostAsync($"http://ladyaev-na.tepk-it.ru/api/updateSouvenir/{souvenirId}",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    actual = (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task EditSouvenir_WithoutToken()
        {
            // Arrange
            int souvenirId = 13;
            int actual = 0;
            int expected = 401;
            try
            {
                // Создание объекта данных для сувенира
                var souvenirData = new
                {
                    name = "ADS",
                    price = 13.00,
                    description = "asda",
                };

                // Сериализация объекта данных в формат JSON
                string jsonData = JsonConvert.SerializeObject(souvenirData);

                // Создание HttpClient для отправки запроса
                using (HttpClient client = new HttpClient())
                {
                    // Добавление токена в заголовок запроса


                    // Добавление типа содержимого в заголовок
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Отправка запроса на сервер
                    HttpResponseMessage response = await client.PostAsync($"http://ladyaev-na.tepk-it.ru/api/updateSouvenir/{souvenirId}",
                        new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // Проверка успешности запроса
                    actual = (int)response.StatusCode;
                    Assert.AreEqual(expected, actual);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
      
    }
    [TestClass]
    public class Employeelist
    {
        [TestMethod]
        public async Task Employeelist_Successful_ReturnsTrue()
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
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    try
                    {
                        // Запрос на получение списка пользователей
                        HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/users");
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            actual = true;
                        }
                        else
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            actual = false;
                          
                        }
                    }
                    
                    catch (HttpRequestException ex)
                    {
                    
                        actual = false;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
            }
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task Employeelist_WithoutToken()
        {
            int actual = 0;
            int expected = 500;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Нет авторизации, токен не отправляется

                    // Запрос на получение списка пользователей
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/users");

                    actual = (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
                
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
    [TestClass]
    public class OrderEmployeeTest
    {
        [TestMethod]
        public async Task OrderList_WithoutToken()
        {
            int actual = 0;
            int expected = 500;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Нет авторизации, токен не отправляется

                    // Запрос на получение списка пользователей
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/showCart");

                    actual = (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
               
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }
       
            [TestMethod]
            public async Task OrderList_WithToken()
            {
            // Arrange
            string password = "Ss1@";
            string login = "sasa-ouy";

            bool actual = true;
                bool expected = true;
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Запрос на получение списка пользователей
                    HttpResponseMessage response = await client.GetAsync("http://ladyaev-na.tepk-it.ru/api/showCart");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
                    actual = false;
                }

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

}
