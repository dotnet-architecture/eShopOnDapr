using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Filters;
using Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories;
using Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnDapr.Services.Basket.API.Model;
using Microsoft.eShopOnDapr.Services.Basket.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Microsoft.eShopOnDapr.Services.Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eShopOnDapr - Basket API", Version = "v1" });

                var identityUrlExternal = Configuration.GetValue<string>("IdentityUrlExternal");

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
                                { "basket", "Basket API" }
                            }
                        }
                    }
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            // Prevent mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Audience = "basket-api";
                    options.Authority = Configuration.GetValue<string>("IdentityUrl");
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "basket");
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddDapr();

            services.AddScoped<IEventBus, DaprEventBus>();
            services.AddScoped<OrderStatusChangedToSubmittedIntegrationEventHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IBasketRepository, DaprBasketRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseRouting();
            app.UseCloudEvents();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}