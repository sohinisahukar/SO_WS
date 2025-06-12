using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomCrudApi.Tests.Integration
{
    /// <summary>
    /// Custom WebApplicationFactory for integration tests.
    /// Configures the application for testing with minimal dependencies.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            // Configure for testing with timeouts
            builder.UseSetting("MongoDbSettings:ConnectionString", "mongodb://localhost:27017/?connectTimeoutMS=1000&serverSelectionTimeoutMS=1000");
            builder.UseSetting("MongoDbSettings:DatabaseName", "TestLibrary");
            builder.UseSetting("MongoDbSettings:BooksCollectionName", "TestBooks");
            
            // Configure services for testing
            builder.ConfigureServices(services =>
            {
                // Suppress logging to reduce test noise
                services.AddLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Error);
                });
            });
        }
    }
}
