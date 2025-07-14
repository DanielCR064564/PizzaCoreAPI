using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PizzaCoreAPI.Data
{
    public class PizzaDbContextFactory : IDesignTimeDbContextFactory<PizzaDbContext>
    {
        public PizzaDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("PizzaDb");

            // If connection string is not found, use the fallback that matches appsettings.json
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=localhost;Port=3306;Database=PizzaCoreDb;Uid=root;Pwd=localhost1234;CharSet=utf8mb4;";
            }

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<PizzaDbContext>();
            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 21)), // Use static version instead of AutoDetect
                options => options.EnableRetryOnFailure()
            );

            return new PizzaDbContext(optionsBuilder.Options);
        }
    }
}
