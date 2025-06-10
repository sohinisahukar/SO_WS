using System.Collections.Generic;
using System.Threading.Tasks;
using CustomCrudApi.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace CustomCrudApi.Services
{
    public class BookMysqlRawService
    {
        private readonly string _connectionString;

        public BookMysqlRawService(IOptions<MySqlSettings> settings)
        {
            _connectionString = settings.Value.ConnectionString;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var books = new List<Book>();
            var sql = "SELECT Id, Title, Author, Year FROM Books";

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new Book
                {
                    Id = reader.GetString(0),      // Id is column 0
                    Title = reader.GetString(1),   // Title is column 1  
                    Author = reader.GetString(2),  // Author is column 2
                    Year = reader.GetInt32(3)      // Year is column 3
                });
            }

            return books;
        }

        public async Task<Book?> GetByIdAsync(string id)
        {
            var sql = "SELECT Id, Title, Author, Year FROM Books WHERE Id = @id";

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Book
                {
                    Id = reader.GetString(0),      // Id is column 0
                    Title = reader.GetString(1),   // Title is column 1
                    Author = reader.GetString(2),  // Author is column 2
                    Year = reader.GetInt32(3)      // Year is column 3
                };
            }

            return null;
        }

        public async Task<Book> CreateAsync(Book book)
        {
            var sql = "INSERT INTO Books (Id, Title, Author, Year) VALUES (@id, @title, @author, @year)";

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@id", book.Id);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@year", book.Year);

            await command.ExecuteNonQueryAsync();
            return book;
        }

        public async Task<bool> UpdateAsync(string id, Book book)
        {
            var sql = "UPDATE Books SET Title = @title, Author = @author, Year = @year WHERE Id = @id";

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@year", book.Year);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var sql = "DELETE FROM Books WHERE Id = @id";

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}