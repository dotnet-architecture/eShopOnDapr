var AppName = "Identity.API";
var builder = WebApplication.CreateBuilder();

builder.AddCustomSerilog();
builder.AddCustomMvc();
builder.AddCustomDatabase();
builder.AddCustomIdentity();
builder.AddCustomIdentityServer();
builder.AddCustomAuthentication();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

// This cookie policy fixes login issues with Chrome 80+ using HHTP
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

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
    app.Logger.LogInformation("Seeding database ({ApplicationName})...", AppName);

    // Apply database migration automatically. Note that this approach is not
    // recommended for production scenarios. Consider generating SQL scripts from
    // migrations instead.
    using (var scope = app.Services.CreateScope())
    {
        await SeedData.EnsureSeedData(scope, app.Configuration, app.Logger);
    }

    app.Logger.LogInformation("Starting web host ({ApplicationName})...", AppName);
    app.Run();

    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", AppName);
    return 1;
}
finally
{
    Serilog.Log.CloseAndFlush();
}
