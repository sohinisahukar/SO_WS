// A C# service to connect to MongoDB and implement Get, GetById, Create, Update, Delete methods
using CrudApi_VSCMC.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CrudApi_VSCMC.Services;

public class BookService
{
    private readonly IMongoCollection<Book> _books;

    public BookService(IMongoClient client, MongoDbSettings settings)
    {
        var database = client.GetDatabase(settings.DatabaseName);
        _books = database.GetCollection<Book>(settings.CollectionName);
    }

    public async Task<List<Book>> GetBooksAsync()
    {
        return await _books.Find(book => true).ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;
            
        return await _books.Find(book => book.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateBookAsync(Book book)
    {
        await _books.InsertOneAsync(book);
    }

    public async Task UpdateBookAsync(string id, Book book)
    {
        await _books.ReplaceOneAsync(b => b.Id == id, book);
    }

    public async Task DeleteBookAsync(string id)
    {
        await _books.DeleteOneAsync(b => b.Id == id);
    }
// This service connects to MongoDB using the provided settings and implements methods to interact with the Book collection.
// It includes asynchronous methods for getting all books, getting a book by ID, creating a new book, updating an existing book, and deleting a book by ID.
}