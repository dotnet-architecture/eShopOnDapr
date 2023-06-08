namespace Basket.FunctionalTests;

internal static class HttpClientExtensions
{
    public static HttpClient CreateIdempotentClient(this BasketWebApplicationFactory server)
    {
        var client = server.CreateClient();

        client.DefaultRequestHeaders.Add("X-Request-Id", Guid.NewGuid().ToString());

        return client;
    }
}