// filepath: CustomCrudApi/Models/MySqlSettings.cs
namespace CustomCrudApi.Models;

/// <summary>
/// Configuration settings for MySQL connection.
/// </summary>
public class MySqlSettings
{
    /// <summary>
    /// Gets or sets the MySQL connection string.
    /// </summary>
    public required string ConnectionString { get; set; }
}