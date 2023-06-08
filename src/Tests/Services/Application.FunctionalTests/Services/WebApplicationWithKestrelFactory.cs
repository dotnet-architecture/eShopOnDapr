namespace FunctionalTests.Services;

public class WebApplicationWithKestrelFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private IHost? _host;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host for TestServer now before we
        // modify the builder to use Kestrel instead.
        var testHost = builder.Build();
        testHost.Start();

        // Modify the host builder to use Kestrel instead
        // of TestServer so we can listen on a real address.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

        // Create and start the Kestrel server before the test server,
        // otherwise due to the way the deferred host builder works
        // for minimal hosting, the server will not get "initialized
        // enough" for the address it is listening on to be available.
        // See https://github.com/dotnet/aspnetcore/issues/33846.
        _host = builder.Build();
        _host.Start();

        // Ensure Dapr is ready
        using var scope = _host.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        using var daprClient = new DaprClientBuilder()
            .UseHttpEndpoint(configuration["DAPR_HTTP_ENDPOINT"])
            .Build();
        while (true)
        {
            var isDaprReady = daprClient.CheckHealthAsync().Result;

            if (isDaprReady)
            {
                break;
            }

            Task.Delay(1000).Wait();
        }

        // Return the host that uses TestServer, rather than the real one.
        // Otherwise the internals will complain about the host's server
        // not being an instance of the concrete type TestServer.
        // See https://github.com/dotnet/aspnetcore/pull/34702.
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
    }
}