var builder = WebApplication.CreateBuilder();

builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomMvc();
builder.AddCustomAuthentication();
builder.AddCustomAuthorization();
builder.AddCustomCors();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API V1");
        c.OAuthClientId("basketswaggerui");
        c.OAuthAppName("Basket Swagger UI");
    });
}

app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");
app.MapDefaultControllerRoute();
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
