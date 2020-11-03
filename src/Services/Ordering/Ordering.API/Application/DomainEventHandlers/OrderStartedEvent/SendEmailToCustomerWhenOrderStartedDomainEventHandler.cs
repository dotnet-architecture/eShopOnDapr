using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    public class SendEmailToCustomerWhenOrderStartedDomainEventHandler
                   : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILogger<SendEmailToCustomerWhenOrderStartedDomainEventHandler> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _daprHttpPort;

        public SendEmailToCustomerWhenOrderStartedDomainEventHandler(ILoggerFactory loggerFactory, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500";

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _logger = loggerFactory.CreateLogger<SendEmailToCustomerWhenOrderStartedDomainEventHandler>();
        }

        public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var payload = new
            {
                metadata = new
                {
                    emailFrom = "eShopOn@dapr.io",
                    emailTo = notification.UserName,
                    subject = $"Your eShopOnDapr Order #{notification.Order.Id}",
                },
                data = CreateEmailBody(notification),
                operation = "create"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending order confirmation email for order {OrderId} to {UserName}.",
                notification.Order.Id, notification.UserName);

            try
            {
                var httpClient = _clientFactory.CreateClient();
                var response = await httpClient.PostAsync($"http://localhost:{_daprHttpPort}/v1.0/bindings/sendmail", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Non success HTTP status-code ({response.StatusCode}) while sending " +
                        $"order confirmation email. Reason: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order confirmation email.");
            }
        }

        private string CreateEmailBody(OrderStartedDomainEvent notification)
        {
            var body = new StringBuilder();

            body.AppendLine($"Dear {notification.UserName},");
            body.AppendLine();
            body.AppendLine($"Thank you for your order. The order id is: {notification.Order.Id}.");
            body.AppendLine();
            body.AppendLine("To follow the status of your order:");
            body.AppendLine();
            body.AppendLine("  1. log into the eShopOnDapr website,");
            body.AppendLine("  2. hover your mouse cursor over your account icon in the top-right corner,");
            body.AppendLine("  3. select 'My Orders' in the context-menu that appears,");
            body.AppendLine($"  4. click the 'Details' link for order with id {notification.Order.Id}.");
            body.AppendLine();
            body.AppendLine("Greetings,");
            body.AppendLine("The eShopOnDapr Team");

            return body.ToString();
        }
    }
}
