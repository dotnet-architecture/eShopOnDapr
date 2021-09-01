using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace eShopOnDapr.BlazorClient
{
    public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public ApiAuthorizationMessageHandler(
            IAccessTokenProvider provider,
            NavigationManager navigation,
            IOptions<Settings> settings)
            : base(provider, navigation)
        {
            // Configure this message handler to attach bearer tokens to
            // requests going to the API Gateway.
            ConfigureHandler(
                authorizedUrls: new[] { settings.Value.ApiGatewayUrl });
        }
    }
}
