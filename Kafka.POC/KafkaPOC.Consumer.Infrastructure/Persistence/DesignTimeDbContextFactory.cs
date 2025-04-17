
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KafkaPOC.Consumer.Infrastructure.Persistence
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../KafkaPOC.Consumer.API"))
                           .AddJsonFile("appsettings.json", optional: false)
                           .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnectionString"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}