// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using Serilog;

public static class ProgramExtensions
{
    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl)
            .Enrich.WithProperty("ApplicationName", "Payment.API")
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "eShopOnDapr - Payment API", Version = "v1" });
        });

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment.API V1");
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr();

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<PaymentSettings>(builder.Configuration);

        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddScoped<OrderStatusChangedToValidatedIntegrationEventHandler>();
    }
}
