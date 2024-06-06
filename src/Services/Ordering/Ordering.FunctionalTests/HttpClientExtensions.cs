namespace Ordering.FunctionalTests;

internal static class HttpClientExtensions
{
    public static HttpClient CreateIdempotentClient(this OrderingWebApplicationFactory server)
    {
        var client = server.CreateClient();

        client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());

        return client;
    }
}