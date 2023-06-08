namespace FunctionalTests.Extensions;

internal static class HttpClientExtensions
{
    public static HttpClient CreateIdempotentClient(this BasketWebApplicationFactory server)
    {
        var client = server.CreateClient();

        client.DefaultRequestHeaders.Add("X-Request-Id", AutoAuthorizeMiddleware.IDENTITY_ID);

        return client;
    }

    public static HttpClient CreateIdempotentClient(this OrderingWebApplicationFactory server)
    {
        var client = server.CreateClient();

        client.DefaultRequestHeaders.Add("x-requestid", AutoAuthorizeMiddleware.IDENTITY_ID);

        return client;
    }
}