using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using eShopOnDapr.BlazorClient.Basket;
using eShopOnDapr.BlazorClient.Catalog;

namespace eShopOnDapr.BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient<CatalogClient>(client => client.BaseAddress = new Uri("http://localhost:5202/c/api/v1/catalog/"));
            builder.Services.AddHttpClient<BasketClient>(client => client.BaseAddress = new Uri("http://localhost:5202/b/api/v1/basket/"));

//            builder.Services.AddSingleton<StatefulBasket>(new StatefulBasket(new LocalStorageBasketStateProvider());

            builder.Services.AddOidcAuthentication<ApplicationAuthenticationState>(options =>
            {
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });

            // Initialize shopping basket. TODO
            //using var serviceProvider = builder.Services.BuildServiceProvider();
            //var basketClient = serviceProvider.GetRequiredService<BasketClient>();
            //await basketClient.InitializeAsync();

            await builder.Build().RunAsync();
        }
    }
}
