// filepath: CustomCrudApi/Data/MySqlDbContext.cs
using CustomCrudApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomCrudApi.Data;
public class MySqlDbContext : DbContext
{
  public MySqlDbContext(DbContextOptions<MySqlDbContext> opts) : base(opts) { }

  public DbSet<Book> Books { get; set; } = null!;
}