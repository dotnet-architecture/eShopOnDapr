using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    public class OrderingContextSeed
    {
        public async Task SeedAsync(OrderingContext context, IWebHostEnvironment env, IOptions<OrderingSettings> settings, ILogger<OrderingContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderingContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;

                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    //if (!context.OrderStatus.Any())
                    //{
                    //    context.OrderStatus.AddRange(GetPredefinedOrderStatus());
                    //}

                    await context.SaveChangesAsync();
                }
            });
        }

        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return new List<CardType>()
            {
                CardType.Amex,
                CardType.Visa,
                CardType.MasterCard
            };
        }

        //private IEnumerable<OrderStatusState> GetPredefinedOrderStatus()
        //{
        //    return new List<OrderStatusState>()
        //    {
        //        OrderStatusState.Submitted,
        //        OrderStatusState.AwaitingStockValidation,
        //        OrderStatusState.Validated,
        //        OrderStatusState.Paid,
        //        OrderStatusState.Shipped,
        //        OrderStatusState.Cancelled
        //    };
        //}

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderingContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
