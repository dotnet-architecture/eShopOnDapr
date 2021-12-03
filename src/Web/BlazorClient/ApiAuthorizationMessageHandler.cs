namespace Microsoft.eShopOnDapr.BlazorClient;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        Settings settings)
        : base(provider, navigation)
    {
        // Configure this message handler to attach bearer tokens to
        // requests going to the API Gateway.
        ConfigureHandler(
            authorizedUrls: new[] { settings.ApiGatewayUrlExternal });
    }
}
