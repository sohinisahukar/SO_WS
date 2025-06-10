
using Xunit;
using Moq;
using MongoDB.Driver;
using RawCrudApi.Models;
using RawCrudApi.Services;

namespace RawCrudApi.Tests.Unit;

public class BookServiceTests
{
    private readonly Mock<IMongoCollection<Book>> _mockCollection;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockCollection = new Mock<IMongoCollection<Book>>();
        _bookService = new BookService(_mockCollection.Object);
    }

    [Fact]
    public void Create_Should_Call_InsertOne()
    {
        var book = new Book { Title = "New Book", Author = "Author", Year = 2024 };
        _bookService.Create(book);
        _mockCollection.Verify(x => x.InsertOne(book, null, default), Times.Once);
    }

    [Fact]
    public void Get_Should_Return_All_Books()
    {
        var books = new List<Book> { new Book { Title = "Book A" } };
        var mockCursor = new Mock<IAsyncCursor<Book>>();
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                  .Returns(true).Returns(false);
        mockCursor.Setup(x => x.Current).Returns(books);

        _mockCollection.Setup(x => x.FindSync(
            It.IsAny<FilterDefinition<Book>>(),
            It.IsAny<FindOptions<Book, Book>>(),
            It.IsAny<CancellationToken>()
        )).Returns(mockCursor.Object);

        var result = _bookService.Get();
        Assert.Single(result);
        Assert.Equal("Book A", result[0].Title);
    }

    [Fact]
    public void Get_ById_Should_Return_Book()
    {
        var book = new Book { Id = "123", Title = "Book 123" };
        var mockCursor = new Mock<IAsyncCursor<Book>>();
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                  .Returns(true).Returns(false);
        mockCursor.Setup(x => x.Current).Returns(new List<Book> { book });

        _mockCollection.Setup(x => x.FindSync(
            It.IsAny<FilterDefinition<Book>>(),
            It.IsAny<FindOptions<Book, Book>>(),
            It.IsAny<CancellationToken>()
        )).Returns(mockCursor.Object);

        var result = _bookService.Get("123");
        Assert.NotNull(result);
        Assert.Equal("Book 123", result.Title);
    }

    [Fact]
    public void Update_Should_Call_ReplaceOne()
    {
        var book = new Book { Id = "456", Title = "Updated Book" };
        _bookService.Update("456", book);
        _mockCollection.Verify(x => x.ReplaceOne(
            It.IsAny<FilterDefinition<Book>>(),
            book,
            It.IsAny<ReplaceOptions>(),
            default
        ), Times.Once);
    }

    [Fact]
    public void Delete_Should_Call_DeleteOne()
    {
        _bookService.Delete("789");
        _mockCollection.Verify(x => x.DeleteOne(
            It.IsAny<FilterDefinition<Book>>(),
            default
        ), Times.Once);
    }
}
