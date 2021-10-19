using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Catalog.API
{
    public class Program
    {
        private const string AppName = "Catalog.API";

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

                Log.Information("Applying database migrations ({ApplicationName})...", AppName);

                // Apply database migration automatically. Note that this approach is not
                // recommended for production scenarios. Consider generating SQL scripts from
                // migrations instead.
                using (var scope = host.Services.CreateScope())
                {
                    var retryPolicy = CreateRetryPolicy(configuration, Log.Logger);
                    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

                    retryPolicy.Execute(context.Database.Migrate);
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

        private static Policy CreateRetryPolicy(IConfiguration configuration, ILogger logger)
        {
            var retryMigrations = false;
            bool.TryParse(configuration["RetryMigrations"], out retryMigrations);

            // Only use a retry policy if configured to do so.
            // When running in an orchestrator/K8s, it will take care of restarting failed services.
            if (retryMigrations)
            {
                return Policy.Handle<Exception>().
                    WaitAndRetryForever(
                        sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                        onRetry: (exception, retry, timeSpan) =>
                        {
                            logger.Warning(
                                exception,
                                "Exception {ExceptionType} with message {Message} detected during database migration (retry attempt {retry})",
                                exception.GetType().Name,
                                exception.Message,
                                retry);
                        }
                    );
            }

            return Policy.NoOp();
        }
    }
}
