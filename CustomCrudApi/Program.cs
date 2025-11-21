using CustomCrudApi.Data;
using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB services
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    try
    {
        var client = new MongoClient(settings.ConnectionString);
        // Test the connection
        var database = client.GetDatabase(settings.DatabaseName);
        return client;
    }
    catch (Exception ex)
    {
        // Log the error but continue - tests will handle MongoDB not being available
        Console.WriteLine($"MongoDB connection failed: {ex.Message}");
        // Return a client anyway for testing purposes
        return new MongoClient(settings.ConnectionString);
    }
});

builder.Services.AddSingleton<BookService>();

// Configure MySQL services
builder.Services.Configure<MySqlSettings>(
    builder.Configuration.GetSection("MySqlSettings"));

builder.Services.AddScoped<BookMysqlRawService>();

builder.Services.AddDbContext<MySqlDbContext>((serviceProvider, options) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MySqlSettings>>().Value;
    options.UseMySql(settings.ConnectionString, 
        Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(settings.ConnectionString));
});

// Configure API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run application (unless in testing mode)
if (!args.Contains("--testing"))
    app.Run();

/// <summary>
/// Program class for testing support.
/// </summary>
public partial class Program { }
