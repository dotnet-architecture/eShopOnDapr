var appName = "Ordering API";
var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomAuthentication();
builder.AddCustomAuthorization();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();
builder.AddCustomDatabase();

builder.Services.AddDaprClient(b => b.UseEndpoints(builder.Configuration));
builder.Services.AddControllers();
builder.Services.AddActors(options =>
{
    options.UseEndpoint(builder.Configuration);
    options.Actors.RegisterActor<OrderingProcessActor>();
});
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseCloudEvents();

using var scope = app.Services.CreateScope();
{
    scope.ServiceProvider.GetRequiredService<IAuthMiddleware>().UseAuth(app);
}
app.UseAuthorization();

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();
app.MapActorsHandlers();
app.MapSubscribeHandler();
app.MapCustomHealthChecks("/hc", "/liveness", UIResponseWriter.WriteHealthCheckUIResponse);
app.MapHub<NotificationsHub>("/hub/notificationhub",
    options => options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);

try
{
    app.Logger.LogInformation("Applying database migration ({ApplicationName})...", appName);
    app.ApplyDatabaseMigration();

    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
}
finally
{
    Serilog.Log.CloseAndFlush();
}

public partial class Program
{
}