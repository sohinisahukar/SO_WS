using CustomCrudApi.Models;
using CustomCrudApi.Services;
using CustomCrudApi.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace CustomCrudApi.Tests.Unit
{
    public class BookServiceTests
    {
        private readonly Mock<IMongoCollection<Book>> _mockCollection;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDatabase> _mockDb;
        private readonly BookService _bookService;
        private readonly List<Book> _books;

        public BookServiceTests()
        {
            // Setup test data
            _books = new List<Book>
            {
                new Book { Id = "507f1f77bcf86cd799439011", Title = "Test Book 1", Author = "Author 1", Year = 2020 },
                new Book { Id = "507f1f77bcf86cd799439012", Title = "Test Book 2", Author = "Author 2", Year = 2021 }
            };

            // Setup MongoDB mocks
            _mockCollection = new Mock<IMongoCollection<Book>>();
            _mockDb = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();

            // Setup mock chain
            _mockClient
                .Setup(c => c.GetDatabase(It.IsAny<string>(), null))
                .Returns(_mockDb.Object);

            _mockDb
                .Setup(d => d.GetCollection<Book>(It.IsAny<string>(), null))
                .Returns(_mockCollection.Object);

            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "TestDb",
                BooksCollectionName = "Books"
            });

            _bookService = new BookService(settings, _mockClient.Object);

            // Setup default FindAsync behavior
            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<FindOptions<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AsyncCursorMock<Book>(_books));
        }

        [Fact]
        public async Task GetAsync_ReturnsAllBooks()
        {
            // Act
            var result = await _bookService.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Book 1", result[0].Title);
        }

        [Fact]
        public async Task GetAsync_WithValidId_ReturnsBook()
        {
            // Arrange
            var expectedBook = _books[0];
            
            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<FindOptions<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AsyncCursorMock<Book>(new[] { expectedBook }));

            // Act
            var result = await _bookService.GetAsync(expectedBook.Id ?? "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBook.Id, result.Id);
            Assert.Equal(expectedBook.Title, result.Title);
        }

        [Fact]
        public async Task CreateAsync_InsertsBook()
        {
            // Arrange
            var newBook = new Book { Title = "New Book", Author = "New Author", Year = 2023 };

            _mockCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<Book>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _bookService.CreateAsync(newBook);

            // Assert
            _mockCollection.Verify(
                c => c.InsertOneAsync(
                    It.Is<Book>(b => b.Title == newBook.Title),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingBook()
        {
            // Arrange
            var existingId = "507f1f77bcf86cd799439011";
            var updatedBook = new Book
            {
                Id = existingId,
                Title = "Updated Book",
                Author = "Updated Author",
                Year = 2024
            };

            _mockCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<Book>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, existingId));

            // Act
            await _bookService.UpdateAsync(existingId, updatedBook);

            // Assert
            _mockCollection.Verify(
                c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.Is<Book>(b => b.Title == updatedBook.Title),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_DeletesBook()
        {
            // Arrange
            var idToDelete = "507f1f77bcf86cd799439011";

            _mockCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            await _bookService.RemoveAsync(idToDelete);

            // Assert
            _mockCollection.Verify(
                c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
