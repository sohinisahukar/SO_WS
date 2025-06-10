// Unit test class for BookService using Moq and xUnit
// Test all methods: Get, Get(id), Create, Update, Delete
using CrudApi_VSCMC.Models;
using CrudApi_VSCMC.Services;
using MongoDB.Driver;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;

namespace CrudApi_VSCMC.Tests.Unit;

public class BookServiceTests
{
    private readonly Mock<IMongoCollection<Book>> _bookCollectionMock;
    private readonly Mock<IMongoClient> _mockClient;
    private readonly Mock<IMongoDatabase> _mockDb;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        // Setup mocks
        _bookCollectionMock = new Mock<IMongoCollection<Book>>();
        _mockClient = new Mock<IMongoClient>();
        _mockDb = new Mock<IMongoDatabase>();

        // Setup mock chain
        _mockClient
            .Setup(c => c.GetDatabase(It.IsAny<string>(), null))
            .Returns(_mockDb.Object);

        _mockDb
            .Setup(d => d.GetCollection<Book>(It.IsAny<string>(), null))
            .Returns(_bookCollectionMock.Object);

        // Create settings
        var settings = new MongoDbSettings
        {
            ConnectionString = "mongodb://fake",
            DatabaseName = "fake",
            CollectionName = "fake"
        };

        // Create service with mocked client
        _bookService = new BookService(_mockClient.Object, settings);
    }

    [Fact]
    public async Task GetBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Id = ObjectId.GenerateNewId().ToString(), Title = "Book 1", Author = "Author 1" },
            new Book { Id = ObjectId.GenerateNewId().ToString(), Title = "Book 2", Author = "Author 2" }
        };

        var mockAsyncCursor = new Mock<IAsyncCursor<Book>>();
        mockAsyncCursor.Setup(x => x.Current).Returns(books);
        mockAsyncCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _bookCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Book>>(),
                It.IsAny<FindOptions<Book>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAsyncCursor.Object);

        // Act
        var result = await _bookService.GetBooksAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook()
    {
        // Arrange
        var book = new Book 
        { 
            Id = ObjectId.GenerateNewId().ToString(), 
            Title = "Test Book", 
            Author = "Test Author" 
        };

        var mockCursor = new Mock<IAsyncCursor<Book>>();
        mockCursor.Setup(x => x.Current).Returns(new List<Book> { book });
        mockCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _bookCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Book>>(),
                It.IsAny<FindOptions<Book>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _bookService.GetBookByIdAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal(book.Title, result.Title);
    }

    [Fact]
    public async Task CreateBookAsync_AddsBook()
    {
        // Arrange
        var book = new Book { Id = "1", Title = "Book 1", Author = "Author 1" };

        // Act
        await _bookService.CreateBookAsync(book);

        // Assert
        _bookCollectionMock.Verify(x => x.InsertOneAsync(book, null, default), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesBook()
    {
        // Arrange
        var book = new Book { Id = ObjectId.GenerateNewId().ToString(), Title = "Book 1", Author = "Author 1" };

        // Act
        await _bookService.UpdateBookAsync(book.Id, book);

        // Assert
        _bookCollectionMock.Verify(x => x.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Book>>(),
            book,
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_DeletesBook()
    {
        // Arrange
        var book = new Book { Id = "1", Title = "Book 1", Author = "Author 1" };

        // Act
        await _bookService.DeleteBookAsync("1");

        // Assert
        _bookCollectionMock.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Book>>(), default), Times.Once);
    }
}