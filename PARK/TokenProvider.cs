using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARK
{
    public class TokenProvider : ITokenProvider
    {
        public async Task<string> GetTokenAsync()
        {
            // Получение токена асинхронно
            return await Task.Run(() =>
            {
                // Здесь может быть ваша логика для получения токена
                // Например, возвращение токена из настроек, если он уже был сохранен
                return Token.token;
            });
        }

        // Реализация метода интерфейса ITokenProvider
        public string GetToken()
        {
            // Вызов асинхронного метода GetTokenAsync и ожидание его завершения с помощью .Result
            return GetTokenAsync().Result;
        }
    }
}