using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using PARK.Pages;
using System.Net.Http;
using PARK;
using System.Windows.Controls;
using System.Net;

namespace CityParkUnitTestProject
{
    [TestClass]
    public class AuthorizationPageTests
    {
        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var authorizationPage = new AutorizationPage(null); // В данном тесте не требуется передавать MainWindow, поэтому используется null
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

            // Arrange
            var authorizationPage = new AddEmployeePage(null);
            authorizationPage.LoginText = "validLogin";
            authorizationPage.Password = "validPassword1+";
            authorizationPage.NameText = "John";
            authorizationPage.SurnameText = "Doe";
            authorizationPage.PatronymicText = "Smith";
            authorizationPage.PhoneText = "123456789";
            authorizationPage.SelectedRoleId = 1;
            // Act
            bool isSuccess = await authorizationPage.RegisterUser(Token.token);
            // Assert
            Assert.IsFalse(isSuccess);
        }
    }
    [TestClass]
    public class IncomeTests
    {

        [TestMethod]
        public async Task ProfitPageTests()
        {

            // Arrange
           var authorizationPage = new ProfitPage(null);
            var startDate = new DateTime(2024, 04, 29, 10, 21, 09);
            var endDate = new DateTime(2024, 05, 04, 11, 12, 31);
            // Act
            string isSuccess = await authorizationPage.GetIncomeForPeriod(startDate, endDate);
            // Assert
            Assert.AreEqual("7040", isSuccess);
        }



    }
       
}
