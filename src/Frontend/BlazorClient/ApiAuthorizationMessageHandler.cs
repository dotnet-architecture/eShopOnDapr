using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace eShopOnDapr.BlazorClient
{
    public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public ApiAuthorizationMessageHandler(
            IAccessTokenProvider provider, NavigationManager navigation)
            : base(provider, navigation)
        {
            // TODO Inject Iconfiguration to read from config file

            // Configure this message handler to attach bearer tokens to
            // requests going to the API Gateway.
            ConfigureHandler(
                authorizedUrls: new[] { "http://localhost:5202/" });
        }
    }
}
