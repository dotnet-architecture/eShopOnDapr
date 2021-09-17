using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Toast;
using eShopOnDapr.BlazorClient.Basket;
using eShopOnDapr.BlazorClient.Catalog;
using eShopOnDapr.BlazorClient.Ordering;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopOnDapr.BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<ApiAuthorizationMessageHandler>();

            var settings = new Settings();
            builder.Configuration.Bind(settings);

            builder.Services.AddHttpClient<CatalogClient>(
                client => client.BaseAddress = new Uri(settings.ApiGatewayUrl));

            builder.Services.AddHttpClient<BasketClient>(
                client => client.BaseAddress = new Uri(settings.ApiGatewayUrl))
                .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<OrderClient>(
                client => client.BaseAddress = new Uri(settings.ApiGatewayUrl))
                .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

            builder.Services
                .AddBlazoredToast()
                .AddScoped<UserBasket>();

            builder.Services.Configure<Settings>(builder.Configuration);

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("OidcAuthentication", options.ProviderOptions);

                options.AuthenticationPaths.LogOutSucceededPath = "";
            });

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
