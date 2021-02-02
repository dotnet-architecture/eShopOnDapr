using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Factories
{
    public class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingContext>
    {
        public OrderingContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("Ordering.API"));

            return new OrderingContext(optionsBuilder.Options);
        }
    }
}