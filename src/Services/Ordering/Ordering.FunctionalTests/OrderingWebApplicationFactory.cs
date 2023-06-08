namespace Ordering.FunctionalTests;

public class OrderingWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ApiUrlBase = "api/v1/orders";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Added to avoid the Authorize data annotation in test environment.
            // Property "SuppressCheckForUnhandledSecurityMetadata" in appsettings.json
            services.Configure<RouteOptions>(context.Configuration);
        });

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

        public static string ShipOrder(int id)
        {
            return $"{ApiUrlBase}/{id}/ship";
        }
    }
}