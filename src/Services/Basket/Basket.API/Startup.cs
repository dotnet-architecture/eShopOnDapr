using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
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

        // This method gets called by the runtime. Use this method to add services to the container.
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
                    options.Audience = "basket";
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

            var healthChecksBuilder = services.AddHealthChecks();
            healthChecksBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            services.AddScoped<IEventBus, DaprEventBus>();
            services.AddTransient<OrderStatusChangedToSubmittedIntegrationEventHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IBasketRepository, DaprBasketRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO Not needed???
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
                    c.OAuthClientId("basketswaggerui");
                    c.OAuthAppName("Basket Swagger UI");
                });
            }

            app.UseRouting();
            app.UseCloudEvents();

            app.UseAuthentication();
            app.UseAuthorization();

//            app.UseStaticFiles();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapDefaultControllerRoute();
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

    //public static class CustomExtensionMethods
    //{
    //    public static IServiceCollection AddSwagger(this IServiceCollection services)
    //    {
    //        services.AddSwaggerGen(c =>
    //        {
    //            c.SwaggerDoc("v1", new OpenApiInfo { Title = "eShopOnDapr - Basket API", Version = "v1" });

    //            //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //            //{
    //            //    Type = SecuritySchemeType.OAuth2,
    //            //    Flows = new OpenApiOAuthFlows()
    //            //    {
    //            //        Implicit = new OpenApiOAuthFlow()
    //            //        {
    //            //            AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
    //            //            TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
    //            //            Scopes = new Dictionary<string, string>()
    //            //            {
    //            //                { "basket", "Basket API" }
    //            //            }
    //            //        }
    //            //    }
    //            //});

    //            //c.OperationFilter<AuthorizeCheckOperationFilter>();
    //        });

    //        return services;
    //    }

    //    private void ConfigureAuthService(IServiceCollection services)
    //    {
    //        // prevent from mapping "sub" claim to nameidentifier.
    //        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    //        var identityUrl = Configuration.GetValue<string>("IdentityUrl");

    //        services.AddAuthentication(options =>
    //        {
    //            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    //        }).AddJwtBearer(options =>
    //        {
    //            options.Authority = identityUrl;
    //            options.RequireHttpsMetadata = false;
    //            options.Audience = "basket";
    //        });
    //    }

    //    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    //    {
    //        // prevent from mapping "sub" claim to nameidentifier.
    //        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    //        var identityUrl = Configuration.GetValue<string>("IdentityUrl");

    //        services.AddAuthentication(options =>
    //        {
    //            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    //        }).AddJwtBearer(options =>
    //        {
    //            options.Authority = identityUrl;
    //            options.RequireHttpsMetadata = false;
    //            options.Audience = "basket";
    //        });

    //        //services.AddAuthentication("Bearer")
    //        //    .AddJwtBearer("Bearer", options =>
    //        //    {
    //        //        options.Authority = "https://localhost:5001";

    //        //        options.TokenValidationParameters = new TokenValidationParameters
    //        //        {
    //        //            ValidateAudience = false
    //        //        };
    //        //    });

    //        //services.AddAuthorization(options =>
    //        //{
    //        //    options.AddPolicy("ApiScope", policy =>
    //        //    {
    //        //        policy.RequireAuthenticatedUser();
    //        //        policy.RequireClaim("scope", "api1");
    //        //    });
    //        //});

    //        return services;
    //    }

    //    public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
    //    {
    //        var hcBuilder = services.AddHealthChecks();

    //        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

    //        // TODO Ideally replace with Dapr health checks
    //        // hcBuilder
    //        //     .AddRedis(
    //        //         configuration["ConnectionString"],
    //        //         name: "redis-check",
    //        //         tags: new string[] { "redis" });

    //        return services;
    //    }

    //    public static IServiceCollection AddEventBus(this IServiceCollection services)
    //    {
    //        services.AddScoped<IEventBus, DaprEventBus>();
    //        services.AddTransient<OrderStatusChangedToSubmittedIntegrationEventHandler>();

    //        return services;
    //    }
    //}
}
