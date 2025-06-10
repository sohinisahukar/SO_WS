using RawCrudApi.Models;
using MongoDB.Driver;

namespace RawCrudApi.Services;

public class BookService
{
    private readonly IMongoCollection<Book> _books;

    public BookService(IConfiguration config)
    {
        var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
        var client = new MongoClient(settings!.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        _books = db.GetCollection<Book>(settings.CollectionName);
    }

    public BookService(IMongoCollection<Book> books)
    {
        _books = books;
    }

    public List<Book> Get() => _books.Find(_ => true).ToList();

    public Book? Get(string id) => _books.Find(b => b.Id == id).FirstOrDefault();

    public void Create(Book book) => _books.InsertOne(book);

    public void Update(string id, Book bookIn) => _books.ReplaceOne(b => b.Id == id, bookIn);

    public void Delete(string id) => _books.DeleteOne(b => b.Id == id);
}
