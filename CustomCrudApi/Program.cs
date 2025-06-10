using CustomCrudApi.Data;
using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
builder.Services.Configure<MongoDbSettings>(
  builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
  var s = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
  return new MongoClient(s.ConnectionString);
});
builder.Services.AddSingleton<BookService>();

// MySQL
builder.Services.Configure<MySqlSettings>(
    builder.Configuration.GetSection("MySqlSettings"));
builder.Services.AddScoped<BookMysqlRawService>();

builder.Services.AddDbContext<MySqlDbContext>( (sp, opts) =>
{
  var s = sp.GetRequiredService<IOptions<MySqlSettings>>().Value;
  opts.UseMySql(s.ConnectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(s.ConnectionString));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Fixed capitalization
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (!args.Contains("--testing"))
    app.Run();

public partial class Program { }
