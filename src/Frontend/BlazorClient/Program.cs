using System;
using System.Net.Http;
using System.Threading.Tasks;
using eShopOnDapr.BlazorClient.Basket;
using eShopOnDapr.BlazorClient.Catalog;
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

            builder.Services.AddHttpClient<CatalogClient>(
                client => client.BaseAddress = new Uri("http://localhost:5202/c/api/v1/catalog/"));

            builder.Services.AddHttpClient<ApiBasketClient>(
                client => client.BaseAddress = new Uri("http://localhost:5202/b/api/v1/basket/"))
                .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

            builder.Services
                .AddScoped<LocalStorageBasketClient>()
                .AddScoped<CustomerBasket>()
                .AddScoped<UserInfo>();

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("oidc", options.ProviderOptions);

                options.ProviderOptions.DefaultScopes.Add("basket");

                options.AuthenticationPaths.LogOutSucceededPath = "";
            });

            var host = builder.Build();

            var userInfo = host.Services.GetRequiredService<UserInfo>();
            await userInfo.LoadUserInfoAsync();

            var customerBasket = host.Services.GetRequiredService<CustomerBasket>();
            await customerBasket.RefreshAsync();

            await host.RunAsync();
        }
    }
}
