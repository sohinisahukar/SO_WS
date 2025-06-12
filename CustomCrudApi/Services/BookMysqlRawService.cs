using CustomCrudApi.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace CustomCrudApi.Services
{
    /// <summary>
    /// Service for managing book operations with MySQL using raw SQL queries.
    /// </summary>
    public class BookMysqlRawService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookMysqlRawService"/> class.
        /// </summary>
        /// <param name="settings">The MySQL configuration settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when settings is null.</exception>
        public BookMysqlRawService(IOptions<MySqlSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(settings.Value);
            
            _connectionString = settings.Value.ConnectionString;
        }        /// <summary>
        /// Retrieves all books asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation containing a collection of books.</returns>
        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var books = new List<Book>();
            const string sql = "SELECT Id, Title, Author, Year FROM Books";

            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await using var command = new MySqlCommand(sql, connection);
                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    books.Add(new Book
                    {
                        Id = reader.GetString(0),
                        Title = reader.GetString(1),
                        Author = reader.GetString(2),
                        Year = reader.GetInt32(3)
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException("Failed to retrieve books from MySQL database.", ex);
            }

            return books;
        }        /// <summary>
        /// Retrieves a book by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A task that represents the asynchronous operation containing the book if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        public async Task<Book?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));

            const string sql = "SELECT Id, Title, Author, Year FROM Books WHERE Id = @id";

            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    return new Book
                    {
                        Id = reader.GetString(0),
                        Title = reader.GetString(1),
                        Author = reader.GetString(2),
                        Year = reader.GetInt32(3)
                    };
                }
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve book with ID '{id}' from MySQL database.", ex);
            }

            return null;
        }        /// <summary>
        /// Creates a new book asynchronously.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <returns>A task that represents the asynchronous operation containing the created book.</returns>
        /// <exception cref="ArgumentNullException">Thrown when book is null.</exception>
        public async Task<Book> CreateAsync(Book book)
        {
            ArgumentNullException.ThrowIfNull(book);

            const string sql = "INSERT INTO Books (Id, Title, Author, Year) VALUES (@id, @title, @author, @year)";

            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await using var command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id", book.Id);
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@year", book.Year);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException("Failed to create book in MySQL database.", ex);
            }

            return book;
        }        /// <summary>
        /// Updates an existing book asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <param name="book">The updated book data.</param>
        /// <returns>A task that represents the asynchronous operation containing a value indicating whether the update was successful.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when book is null.</exception>
        public async Task<bool> UpdateAsync(string id, Book book)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));

            ArgumentNullException.ThrowIfNull(book);

            const string sql = "UPDATE Books SET Title = @title, Author = @author, Year = @year WHERE Id = @id";

            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await using var command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@year", book.Year);

                var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Failed to update book with ID '{id}' in MySQL database.", ex);
            }
        }        /// <summary>
        /// Removes a book asynchronously.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A task that represents the asynchronous operation containing a value indicating whether the removal was successful.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        public async Task<bool> RemoveAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Book ID cannot be null or empty.", nameof(id));

            const string sql = "DELETE FROM Books WHERE Id = @id";

            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Failed to remove book with ID '{id}' from MySQL database.", ex);
            }
        }
    }
}