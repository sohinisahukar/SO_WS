// filepath: CustomCrudApi/Data/MySqlDbContext.cs
using CustomCrudApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomCrudApi.Data;

/// <summary>
/// Entity Framework Core database context for MySQL operations.
/// </summary>
public class MySqlDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Books DbSet.
    /// </summary>
    public DbSet<Book> Books { get; set; } = null!;
}