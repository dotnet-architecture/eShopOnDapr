using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    public class SendEmailToCustomerWhenOrderStartedDomainEventHandler
                   : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _daprHttpPort;

        public SendEmailToCustomerWhenOrderStartedDomainEventHandler(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
            _daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "50002";
        }

        public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var payload = new
            {
                metadata = new
                {
                    emailFrom = "eShopOn@dapr.io",
                    emailTo = notification.UserId,
                    subject = $"Your eShopOnDapr Order #{notification.Order.Id}",
                },
                data = $"Dear {notification.UserName},\n\n" +
                       $"The status of your order #{notification.Order.Id} " +
                       $"has changed to {notification.Order.OrderStatus}.\n\n" +
                       $"Greetings,\n" +
                       $"The eShopOnDapr team",
                operation = "create"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json");

            var httpClient = _clientFactory.CreateClient();

            await httpClient.PostAsync($"http://localhost:{_daprHttpPort}/v1.0/bindings/sendmail", content);
        }
    }
}
