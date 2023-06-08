extern alias basket;

using basket.Microsoft.eShopOnDapr.Services.Basket.API.Middlewares;

namespace FunctionalTests.Services.Basket;

public class BasketWebApplicationFactory : WebApplicationWithKestrelFactory<basket::Program>
{
    private const string ApiUrlBase = "api/v1/basket";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseConfiguration(new ConfigurationBuilder()
            .AddEnvironmentVariables("ASPNETCORE_BASKET_")
            .Build());

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
        public static string CreateBasket = ApiUrlBase;
        public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
    }
}