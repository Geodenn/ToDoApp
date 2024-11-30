using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;
using ToDoApp.Models;

namespace ToDoApp.Tests.ApiTests
{
    public class ToDoApiTests : IClassFixture<WebApplicationFactory<ToDo>>
    {
        private readonly HttpClient _client;

        public ToDoApiTests(WebApplicationFactory<ToDo> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test_GetAllTodos_ReturnsOk()
        {
            // Wysyłanie zapytania GET do /todos
            var response = await _client.GetAsync("/todos");

            // Sprawdzenie, czy odpowiedź jest poprawna (status 200 OK)
            response.EnsureSuccessStatusCode();

            // Sprawdzenie, czy odpowiedź zawiera poprawny typ zawartości
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Test_GetTodoById_ReturnsOk()
        {
            var response = await _client.GetAsync("/todos/1");  // Zakładając, że zadanie o ID 1 istnieje

            response.EnsureSuccessStatusCode();  // Sprawdzanie, czy status odpowiedzi to 200 OK
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());  // Sprawdzenie typu zawartości
        }

        [Fact]
        public async Task Test_GetTodoById_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/todos/999");  // Zakładając, że zadanie o ID 999 nie istnieje

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);  // Sprawdzenie, czy status to 404 Not Found
        }

        [Fact]
        public async Task Test_GetTodosForToday_ReturnsOk()
        {
            var response = await _client.GetAsync("/todos/today");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Test_GetTodosForTomorrow_ReturnsOk()
        {
            var response = await _client.GetAsync("/todos/tomorrow");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Test_GetTodosForNextWeek_ReturnsOk()
        {
            var response = await _client.GetAsync("/todos/nextweek");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Test_PostTodo_ReturnsCreated()
        {
            //  Tworzenie testowych danych
            var newTodo = new
            {
                Title = "Test Task",
                Description = "Test Description",
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                IsDone = false,
                PercentComplete = 0
            };

            var response = await _client.PostAsJsonAsync("/todos", newTodo);  // Wysłanie zapytania POST

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);  // Sprawdzenie, czy odpowiedź ma status 201 Created
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Task", content);  // Sprawdzenie, czy odpowiedź zawiera informacje dla danego zadania
        }

        [Fact]
        public async Task Test_PutTodo_ReturnsNoContent()
        {
            //  Aktualizowanie testowych danych
            var updatedTodo = new
            {
                Title = "Updated Test Task",
                Description = "Updated Description",
                ExpiryDate = DateTime.UtcNow.AddDays(2),
                IsDone = true,
                PercentComplete = 100
            };

            var response = await _client.PutAsJsonAsync("/todos/2", updatedTodo);  // Zakłając, że zadanie o ID 2 istnieje

            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);  // Sprawdzenie, czy odpowiedź ma status 204 NoContent
        }

        [Fact]
        public async Task Test_DeleteTodo_ReturnsNoContent()
        {
            var response = await _client.DeleteAsync("/todos/1");  // Zakładając, że zadanie o ID 1 istnieje

            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);  // Sprawdzenie, czy odpowiedź ma status 204 NoContent
        }

        [Fact]
        public async Task Test_DeleteTodo_ReturnsNotFound()
        {
            var response = await _client.DeleteAsync("/todos/999");  // Zakłając, że zadanie o ID 999 nie istnieje

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);  // Sprawdzenie, czy odpowiedź ma status 404 Not Found
        }

    }
}
