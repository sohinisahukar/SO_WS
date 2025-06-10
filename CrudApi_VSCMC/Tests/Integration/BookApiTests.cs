// Integration tests for BookController using WebApplicationFactory<Program>
// Test POST /api/books and GET /api/books/{id}
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using CrudApi_VSCMC.Models;
using MongoDB.Bson;

namespace CrudApi_VSCMC.Tests.Integration
{
    public class BookApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostBook_CreatesBook()
        {
            // Arrange
            var book = new Book 
            { 
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "Book 1", 
                Author = "Author 1" 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/books", book);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetBookById_ReturnsBook()
        {
            // Arrange
            var book = new Book 
            { 
                Id = ObjectId.GenerateNewId().ToString(), 
                Title = "Book 1", 
                Author = "Author 1" 
            };
            
            var postResponse = await _client.PostAsJsonAsync("/api/books", book);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await _client.GetAsync($"/api/books/{book.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(result);
            Assert.Equal(book.Id, result.Id);
            Assert.Equal(book.Title, result.Title);
        }
    }
}
// This integration test class uses WebApplicationFactory to create an in-memory test server for the Book API.
// It includes tests for creating a book via POST and retrieving a book by ID via GET.