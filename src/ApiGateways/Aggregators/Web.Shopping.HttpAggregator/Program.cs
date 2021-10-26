using Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator;

var builder = WebApplication.CreateBuilder();

builder.AddCustomSerilog();
builder.AddCustomMvc();
builder.AddCustomAuthentication();
builder.AddCustomApplicationServices();
builder.AddCustomHealthChecks();

var app = builder.Build();

app.UseCustomPathBase();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseCustomSwagger();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapCustomHealthChecks();

await app.RunAsync();

