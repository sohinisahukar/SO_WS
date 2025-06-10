using CrudApi_VSCMC.Models;
using CrudApi_VSCMC.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add this - required for API controllers
builder.Services.AddEndpointsApiExplorer(); // Add this - required for Swagger
builder.Services.AddSwaggerGen(); // Add this - required for Swagger
builder.Services.AddSingleton<MongoDbSettings>(sp =>
    sp.GetRequiredService<IConfiguration>().GetSection("MongoDb").Get<MongoDbSettings>() 
    ?? throw new InvalidOperationException("MongoDb configuration is missing"));
builder.Services.AddSingleton<IMongoClient>(sp => 
{
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

if (!args.Contains("--testing"))
{
    app.Run();
}

public partial class Program 
{ 
    // This empty public partial class is needed for testing
}