using eShopOnContainers.WebSPA;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.IO;
using WebSPA.Infrastructure;

namespace eShopConContainers.WebSPA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Startup()
        {
            var localPath = new Uri(Configuration["ASPNETCORE_URLS"])?.LocalPath ?? "/";
            Configuration["BaseUrl"] = localPath;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterAppInsights(services);

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri(Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });

            services.Configure<AppSettings>(Configuration);

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webspa";
                })
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(Configuration["DPConnectionString"]), "DataProtection-Keys");
            }

            // Add Antiforgery services and configure the header name that angular will use by default.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            // Add controllers support and add a global AutoValidateAntiforgeryTokenFilter that will make the application check for an Antiforgery token on all "mutating" requests (POST, PUT, DELETE).
            // The AutoValidateAntiforgeryTokenFilter is an internal class registered when we register views, so we need to register controllers and views also.
            services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Setup where the compiled version of our spa application will be, when in production. 
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Seed Data
            WebContextSeed.Seed(app, env, loggerFactory);

            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Use the SPA static hosting middleware is the Angular app is already built.
            var useSpaStaticHosting = Directory.Exists("wwwroot");
            if (useSpaStaticHosting)
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            // Handles all still unnatended (by any other middleware) requests by returning the default page of the SPA (wwwroot/index.html).
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                // the root of the angular app. (Where the package.json lives)
                spa.Options.SourcePath = "Client";

                if (!useSpaStaticHosting)
                {
                    // use the SpaServices extension method for angular, that will make the application to run "ng serve" for us, when in development.
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddApplicationInsightsKubernetesEnricher();
        }
    }
}
