namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator;

public static class ProgramExtensions
{
    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .Enrich.WithProperty("ApplicationName", "Catalog.API")
            .CreateLogger();
    }

    public static void AddCustomAuthentication(this WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        var identityUrl = builder.Configuration.GetValue<string>("urls:identity");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "shoppingaggr-api";
        });
    }

    public static void AddCustomMvc(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();

        builder.Services.AddControllers()
            .AddDapr()
            .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Shopping Aggregator for Web Clients",
                Version = "v1",
                Description = "Shopping Aggregator for Web Clients"
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{builder.Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                        TokenUrl = new Uri($"{builder.Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),

                        Scopes = new Dictionary<string, string>()
                        {
                            { "webshoppingagg", "Shopping Aggregator for Web Clients" }
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        builder.Services.AddSwaggerGenNewtonsoftSupport();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
           );
        });
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBasketService, BasketService>(
            _ => new BasketService(DaprClient.CreateInvokeHttpClient("basket-api")));

        builder.Services.AddSingleton<ICatalogService, CatalogService>(
            _ => new CatalogService(DaprClient.CreateInvokeHttpClient("catalog-api")));
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        var healthCheckBuilder = builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddUrlGroup(new Uri(builder.Configuration["CatalogUrlHC"]), name: "catalogapi-check", tags: new string[] { "catalogapi" })
            .AddUrlGroup(new Uri(builder.Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" })
            .AddUrlGroup(new Uri(builder.Configuration["BasketUrlHC"]), name: "basketapi-check", tags: new string[] { "basketapi" });
    }

    public static void UseCustomPathBase(this WebApplication app)
    {
        var pathBase = app.Configuration["PATH_BASE"];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.Logger.LogDebug("Using PATH BASE '{pathBase}'", pathBase);
            app.UsePathBase(pathBase);
        }
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        var pathBase = app.Configuration["PATH_BASE"];

        app.UseSwagger().UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Purchase BFF V1");

            c.OAuthClientId("webshoppingaggswaggerui");
            c.OAuthClientSecret(string.Empty);
            c.OAuthRealm(string.Empty);
            c.OAuthAppName("web shopping bff Swagger UI");
        });
    }

    public static void MapCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }
}