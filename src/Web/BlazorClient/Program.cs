namespace Microsoft.eShopOnDapr.BlazorClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        var settings = await LoadSettingsFromHostAsync(builder.Services);
        builder.Services.AddSingleton(settings);

        builder.Services.AddTransient<ApiAuthorizationMessageHandler>();

        builder.Services.AddHttpClient<CatalogClient>(
            client => client.BaseAddress = new Uri(settings.ApiGatewayUrlExternal));

        builder.Services.AddHttpClient<BasketClient>(
            client => client.BaseAddress = new Uri(settings.ApiGatewayUrlExternal))
            .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

        builder.Services.AddHttpClient<OrderClient>(
            client => client.BaseAddress = new Uri(settings.ApiGatewayUrlExternal))
            .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

        builder.Services
            .AddBlazoredToast()
            .AddScoped<UserBasket>();

        builder.Services.AddOidcAuthentication(options =>
        {
            builder.Configuration.Bind("OidcAuthentication", options.ProviderOptions);

            options.ProviderOptions.Authority = settings.IdentityUrlExternal;
            options.AuthenticationPaths.LogOutSucceededPath = "";
        });

        var host = builder.Build();
        await host.RunAsync();
    }

    private static async Task<Settings> LoadSettingsFromHostAsync(IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        return (await httpClient.GetFromJsonAsync<Settings>("/settings"))!;
    }
}
