using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnDapr.Services.Identity.API.Data;
using Microsoft.eShopOnDapr.Services.Identity.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Identity.API
{
    public class SeedData
    {
        public static void EnsureSeedData(
            IServiceScope scope,
            IConfiguration configuration,
            ILogger logger)
        {
            var retryPolicy = CreateRetryPolicy(configuration, logger);
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            retryPolicy.Execute(() =>
            {
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                        CardHolderName = "Alice Smith",
                        CardNumber = "4012888888881881",
                        CardType = 1,
                        City = "Redmond",
                        Country = "U.S.",
                        Expiration = "12/20",
                        Id = Guid.NewGuid().ToString(),
                        LastName = "Smith",
                        Name = "Alice",
                        PhoneNumber = "1234567890",
                        ZipCode = "98052",
                        State = "WA",
                        Street = "15703 NE 61st Ct",
                        SecurityNumber = "123"
                    };

                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created");
                }
                else
                {
                    Log.Debug("alice already exists");
                }

                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com",
                        EmailConfirmed = true,
                        CardHolderName = "Bob Smith",
                        CardNumber = "4012888888881881",
                        CardType = 1,
                        City = "Redmond",
                        Country = "U.S.",
                        Expiration = "12/20",
                        Id = Guid.NewGuid().ToString(),
                        LastName = "Smith",
                        Name = "Bob",
                        PhoneNumber = "1234567890",
                        ZipCode = "98052",
                        State = "WA",
                        Street = "15703 NE 61st Ct",
                        SecurityNumber = "456"
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }
            });
        }

        private static Policy CreateRetryPolicy(IConfiguration configuration, ILogger logger)
        {
            var retryMigrations = false;
            bool.TryParse(configuration["RetryMigrations"], out retryMigrations);

            // Only use a retry policy if configured to do so.
            // When running in an orchestrator/K8s, it will take care of restarting failed services.
            if (retryMigrations)
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
}
