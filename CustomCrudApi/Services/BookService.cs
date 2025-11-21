using CustomCrudApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomCrudApi.Services
{
    /// <summary>
    /// Service for managing book operations with MongoDB.
    /// </summary>
    public class BookService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="mongoDbSettings">The MongoDB configuration settings.</param>
        /// <param name="mongoClient">The MongoDB client instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public BookService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
        {
            ArgumentNullException.ThrowIfNull(mongoDbSettings);
            ArgumentNullException.ThrowIfNull(mongoClient);
            ArgumentNullException.ThrowIfNull(mongoDbSettings.Value);

            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _booksCollection = mongoDatabase.GetCollection<Book>(mongoDbSettings.Value.BooksCollectionName);
        }

        /// <summary>
        /// Retrieves all books asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation containing a list of books.</returns>
        public async Task<List<Book>> GetAsync()
        {
            return await _booksCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a book by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A task that represents the asynchronous operation containing the book if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        public async Task<Book?> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));

            return await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
        }        /// <summary>
        /// Creates a new book asynchronously.
        /// </summary>
        /// <param name="newBook">The book to create.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when newBook is null.</exception>
        public async Task CreateAsync(Book newBook)
        {
            ArgumentNullException.ThrowIfNull(newBook);
            
            // Generate new ObjectId if not provided
            if (string.IsNullOrEmpty(newBook.Id))
            {
                newBook.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }
            
            await _booksCollection.InsertOneAsync(newBook).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an existing book asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <param name="updatedBook">The updated book data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when updatedBook is null.</exception>
        public async Task UpdateAsync(string id, Book updatedBook)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));
            
            ArgumentNullException.ThrowIfNull(updatedBook);
            
            await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a book asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        public async Task RemoveAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));
            
            await _booksCollection.DeleteOneAsync(x => x.Id == id).ConfigureAwait(false);
        }
    }
}
