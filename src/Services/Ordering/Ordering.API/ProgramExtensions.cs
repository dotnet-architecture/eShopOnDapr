// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Ordering.API;

public static class ProgramExtensions
{
    private const string AppName = "Ordering API";

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
        //builder.Configuration.AddDaprSecretStore(
        //    "secretstore",
        //    new DaprClientBuilder().Build());
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"eShopOnDapr - {AppName}", Version = "v1" });

            var identityUrlExternal = builder.Configuration.GetValue<string>("IdentityUrlExternal");

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = new Dictionary<string, string>()
                            {
                                { "ordering", AppName }
                            }
                    }
                }
            });

            c.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
            c.OAuthClientId("orderingswaggerui");
            c.OAuthAppName("Ordering Swagger UI");
        });
    }

    public static void AddCustomAuthentication(this WebApplicationBuilder builder)
    {
        // Prevent mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.Audience = "ordering-api";
                options.Authority = builder.Configuration.GetValue<string>("IdentityUrl");
                options.RequireHttpsMetadata = false;
            });
    }

    public static void AddCustomAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "ordering");
            });
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:OrderingDB"],
                name: "OrderingDB-check",
                tags: new string[] { "orderdb" });

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IEmailService, EmailService>();

        builder.Services.Configure<OrderingSettings>(builder.Configuration);
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder) =>
        builder.Services.AddDbContext<OrderingDbContext>(
            options => options.UseSqlServer(builder.Configuration["ConnectionStrings:OrderingDB"]));

    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();
        
        var retryPolicy = CreateRetryPolicy(app.Configuration, Log.Logger);
        var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        retryPolicy.Execute(context.Database.Migrate);
    }

    private static Policy CreateRetryPolicy(IConfiguration configuration, Serilog.ILogger logger)
    {
        // Only use a retry policy if configured to do so.
        // When running in an orchestrator/K8s, it will take care of restarting failed services.
        if (bool.TryParse(configuration["RetryMigrations"], out bool retryMigrations))
        {
            return Policy.Handle<Exception>().
                WaitAndRetryForever(
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, retry, timeSpan) =>
                    {
                        logger.Warning(
                            exception,
                            "Exception {ExceptionType} with message {Message} detected during database migration (retry attempt {retry})",
                            exception.GetType().Name,
                            exception.Message,
                            retry);
                    }
                );
        }

        return Policy.NoOp();
    }
}
