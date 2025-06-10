using System.Net;
using System.Net.Http.Json;
using CustomCrudApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CustomCrudApi.Tests.Integration
{
    public class BookApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAndGetBook_Success()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Integration Test Book",
                Author = "Test Author",
                Year = 2025
            };

            // Act - Create
            var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
            
            // Assert - Create
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(created);
            Assert.NotNull(created.Id);
            Assert.Equal(24, created.Id.Length); // MongoDB ObjectId is 24 chars
            Assert.Equal(newBook.Title, created.Title);

            // Act - Get
            var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
            
            // Assert - Get
            getResponse.EnsureSuccessStatusCode();
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal(newBook.Title, retrieved.Title);
        }

        [Fact]
        public async Task GetBook_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/books/507f1f77bcf86cd799439099");
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetBook_WithInvalidIdFormat_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/books/123"); // Not 24 chars
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllBooks_Success()
        {
            // Arrange - Create a book to ensure there's at least one
            var book = new Book
            {
                Title = "Test Book for GetAll",
                Author = "Test Author",
                Year = 2025
            };
            await _client.PostAsJsonAsync("/api/books", book);

            // Act
            var response = await _client.GetAsync("/api/books");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var books = await response.Content.ReadFromJsonAsync<List<Book>>();
            Assert.NotNull(books);
            Assert.NotEmpty(books);
        }

        [Fact]
        public async Task DeleteBook_Success()
        {
            // Arrange - Create a book to delete
            var book = new Book
            {
                Title = "Book to Delete",
                Author = "Delete Author",
                Year = 2025
            };
            var createResponse = await _client.PostAsJsonAsync("/api/books", book);
            var created = await createResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(created?.Id);

            // Act - Delete
            var deleteResponse = await _client.DeleteAsync($"/api/books/{created.Id}");
            
            // Assert - Delete
            deleteResponse.EnsureSuccessStatusCode();

            // Verify - Get should return NotFound
            var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_Success()
        {
            // Arrange - Create a book to update
            var book = new Book
            {
                Title = "Original Title",
                Author = "Original Author",
                Year = 2025
            };
            var createResponse = await _client.PostAsJsonAsync("/api/books", book);
            var created = await createResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(created?.Id);

            // Update the book
            created.Title = "Updated Title";
            
            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/books/{created.Id}", created);
            
            // Assert
            updateResponse.EnsureSuccessStatusCode();

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updated = await getResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
        }
    }
}
