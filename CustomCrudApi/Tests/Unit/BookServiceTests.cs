using CustomCrudApi.Models;
using CustomCrudApi.Services;
using CustomCrudApi.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace CustomCrudApi.Tests.Unit
{
    /// <summary>
    /// Unit tests for the BookService class.
    /// </summary>
    public class BookServiceTests
    {
        private readonly Mock<IMongoCollection<Book>> _mockCollection;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly BookService _bookService;
        private readonly List<Book> _testBooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookServiceTests"/> class.
        /// </summary>
        public BookServiceTests()
        {
            // Setup test data
            _testBooks = new List<Book>
            {
                new Book { Id = "507f1f77bcf86cd799439011", Title = "Test Book 1", Author = "Author 1", Year = 2020 },
                new Book { Id = "507f1f77bcf86cd799439012", Title = "Test Book 2", Author = "Author 2", Year = 2021 }
            };

            // Setup MongoDB mocks
            _mockCollection = new Mock<IMongoCollection<Book>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();

            // Setup mock chain
            _mockClient
                .Setup(c => c.GetDatabase(It.IsAny<string>(), null))
                .Returns(_mockDatabase.Object);

            _mockDatabase
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
                .ReturnsAsync(new AsyncCursorMock<Book>(_testBooks));
        }

        /// <summary>
        /// Tests that GetAsync returns all books.
        /// </summary>
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

        /// <summary>
        /// Tests that GetAsync with valid ID returns the correct book.
        /// </summary>
        [Fact]
        public async Task GetAsync_WithValidId_ReturnsBook()
        {
            // Arrange
            var expectedBook = _testBooks[0];
            
            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Book>>(),
                    It.IsAny<FindOptions<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AsyncCursorMock<Book>(new List<Book> { expectedBook }));

            // Act
            var result = await _bookService.GetAsync(expectedBook.Id!);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBook.Id, result.Id);
            Assert.Equal(expectedBook.Title, result.Title);
        }        /// <summary>
        /// Tests that GetAsync with null or empty ID throws ArgumentException.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.GetAsync(invalidId));
        }

        /// <summary>
        /// Tests that GetAsync with null ID throws ArgumentException.
        /// </summary>
        [Fact]
        public async Task GetAsync_WithNullId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.GetAsync(null!));
        }

        /// <summary>
        /// Tests that CreateAsync inserts a book.
        /// </summary>
        [Fact]
        public async Task CreateAsync_InsertsBook()
        {
            // Arrange
            var newBook = new Book { Id = "507f1f77bcf86cd799439013", Title = "New Book", Author = "New Author", Year = 2023 };

            // Act
            await _bookService.CreateAsync(newBook);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(It.Is<Book>(b => b.Id == newBook.Id), null, default), Times.Once);
        }

        /// <summary>
        /// Tests that CreateAsync with null book throws ArgumentNullException.
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithNullBook_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookService.CreateAsync(null!));
        }

        /// <summary>
        /// Tests that UpdateAsync replaces a book.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ReplacesBook()
        {
            // Arrange
            var bookId = "507f1f77bcf86cd799439011";
            var updatedBook = new Book { Id = bookId, Title = "Updated Book", Author = "Updated Author", Year = 2024 };
            
            var mockResult = new Mock<ReplaceOneResult>();
            _mockCollection
                .Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Book>>(), It.IsAny<Book>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult.Object);

            // Act
            await _bookService.UpdateAsync(bookId, updatedBook);

            // Assert
            _mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Book>>(), 
                It.Is<Book>(b => b.Id == updatedBook.Id), 
                It.IsAny<ReplaceOptions>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that UpdateAsync with invalid parameters throws appropriate exceptions.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithInvalidParameters_ThrowsExceptions()
        {
            var validBook = new Book { Id = "507f1f77bcf86cd799439011", Title = "Valid Book", Author = "Valid Author", Year = 2023 };

            // Test null/empty ID
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.UpdateAsync("", validBook));
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.UpdateAsync("   ", validBook));

            // Test null book
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookService.UpdateAsync("507f1f77bcf86cd799439011", null!));
        }

        /// <summary>
        /// Tests that RemoveAsync deletes a book.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_DeletesBook()
        {
            // Arrange
            var bookId = "507f1f77bcf86cd799439011";
            
            var mockResult = new Mock<DeleteResult>();
            _mockCollection
                .Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Book>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult.Object);

            // Act
            await _bookService.RemoveAsync(bookId);

            // Assert
            _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Book>>(), It.IsAny<CancellationToken>()), Times.Once);
        }        /// <summary>
        /// Tests that RemoveAsync with invalid ID throws ArgumentException.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RemoveAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.RemoveAsync(invalidId));
        }

        /// <summary>
        /// Tests that RemoveAsync with null ID throws ArgumentException.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_WithNullId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.RemoveAsync(null!));
        }
    }
}
