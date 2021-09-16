using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Identity.API
{
    public class Program
    {
        private const string AppName = "Identity.API";

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            var seqServerUrl = configuration["SeqServerUrl"];

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl)
                .Enrich.WithProperty("ApplicationName", AppName)
                .CreateLogger();

            try
            {
                Log.Information("Configuring web host ({ApplicationName})...", AppName);
                var host = CreateHostBuilder(args).Build();

                Log.Information("Seeding database ({ApplicationName})...", AppName);

                // Apply database migration automatically. Note that this approach is not
                // recommended for production scenarios. Consider generating SQL scripts from
                // migrations instead.
                using (var scope = host.Services.CreateScope())
                {
                    SeedData.EnsureSeedData(scope, configuration, Log.Logger);
                }

                Log.Information("Starting web host ({ApplicationName})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly ({ApplicationName})...", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}

