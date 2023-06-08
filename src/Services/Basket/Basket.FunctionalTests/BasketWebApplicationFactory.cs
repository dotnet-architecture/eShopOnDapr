namespace Basket.FunctionalTests;

public class BasketWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ApiUrlBase = "api/v1/basket";

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
        public static string Basket = ApiUrlBase;
    }

    public static class Post
    {
        public static string Basket = ApiUrlBase;
        public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
    }
}