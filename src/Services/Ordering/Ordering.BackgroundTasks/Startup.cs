namespace Ordering.BackgroundTasks
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Ordering.BackgroundTasks.Extensions;
    using Ordering.BackgroundTasks.Tasks;
    using HealthChecks.UI.Client;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomHealthCheck(this.Configuration)
                .AddEventBus()
                .Configure<BackgroundTaskSettings>(this.Configuration)
                .AddOptions()
                .AddHostedService<GracePeriodManagerService>()
                .AddEventBus(this.Configuration)
                .AddDaprClient();
        }


        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddScoped<IEventBus, DaprEventBus>();

            return services;
        }
    }
}
