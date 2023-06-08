extern alias ordering;

using ordering.Microsoft.eShopOnDapr.Services.Ordering.API.Middlewares;

namespace FunctionalTests.Services.Ordering;

public class OrderingWebApplicationFactory : WebApplicationWithKestrelFactory<ordering::Program>
{
    private const string ApiUrlBase = "api/v1/orders";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Give dapr sidecar some time to establish connection with dapr placement
        Task.Delay(4000).Wait();

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseConfiguration(new ConfigurationBuilder()
            .AddEnvironmentVariables("ASPNETCORE_ORDERING_")
            .Build());

        builder.ConfigureTestServices(services =>
        {
            services.AddTransient<IAuthMiddleware, TestAuthMiddleware>();
        });
    }

    public static class Get
    {
        public static string Orders = ApiUrlBase;

        public static string OrderBy(int id)
        {
            return $"{ApiUrlBase}/{id}";
        }
    }

    public static class Put
    {
        public static string CancelOrder(int id)
        {
            return $"{ApiUrlBase}/{id}/cancel";
        }
    }
}