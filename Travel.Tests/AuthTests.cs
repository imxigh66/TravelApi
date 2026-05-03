using FluentAssertions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Tests
{
    public class AuthTests
    {
        private readonly RestClient _client;
        public AuthTests()
        {
            _client = new RestClient("http://localhost:5114");
        }
      

        // TC-001: Вход с правильными данными
        [Fact]
        public async Task Login_ValidCredentials_Returns200WithTokens()
        {
            var request = new RestRequest("/api/auth/login", Method.Post);
            request.AddJsonBody(new
            {
                email = "lara@mail.ru",
                password = "16egurolA"
            });

            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Should().Contain("accessToken");
            response.Content.Should().Contain("refreshToken");
        }

        // TC-002: Неверный пароль
        [Fact]
        public async Task Login_WrongPassword_Returns401()
        {
            var request = new RestRequest("/api/auth/login", Method.Post);
            request.AddJsonBody(new
            {
                email = "lara@mail.ru",
                password = "НеверныйПароль123"
            });

            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            response.Content.Should().Contain("false");
        }

        // TC-003: Несуществующий email
        [Fact]
        public async Task Login_NonExistentEmail_Returns401()
        {
            var request = new RestRequest("/api/auth/login", Method.Post);
            request.AddJsonBody(new
            {
                email = "nevsuwestvuet@test.com",
                password = "16egurolA"
            });

            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        // TC-004: Пустые поля
        [Fact]
        public async Task Login_EmptyFields_Returns400WithValidationErrors()
        {
            var request = new RestRequest("/api/auth/login", Method.Post);
            request.AddJsonBody(new
            {
                email = "",
                password = ""
            });

            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Content.Should().Contain("errors");
        }

        // TC-005: Без токена — защищённый роут
        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_Returns401()
        {
            var request = new RestRequest("/api/trips/5/budget", Method.Get);

            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        // BUG-001: Проверяем что сообщения одинаковые
        [Fact]
        public async Task Login_WrongPasswordAndWrongEmail_SameErrorMessage()
        {
            var request1 = new RestRequest("/api/auth/login", Method.Post);
            request1.AddJsonBody(new { email = "lara@mail.ru", password = "wrongpass" });
            var response1 = await _client.ExecuteAsync(request1);

            var request2 = new RestRequest("/api/auth/login", Method.Post);
            request2.AddJsonBody(new { email = "notexist@mail.com", password = "wrongpass" });
            var response2 = await _client.ExecuteAsync(request2);

            // Этот тест должен УПАСТЬ — баг который мы нашли вручную
            response1.Content.Should().Be(response2.Content,
                "сообщения об ошибке должны быть одинаковыми во избежание User Enumeration");
        }
    }
}
