var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

builder.Services.AddDaprClient();
builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

app.UseCloudEvents();

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();
app.MapSubscribeHandler();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

try
{
    app.Logger.LogInformation("Starting web host...");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly...");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
