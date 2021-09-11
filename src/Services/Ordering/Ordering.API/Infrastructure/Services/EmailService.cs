using System;
using System.Text;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.eShopOnDapr.Services.Ordering.API.Model;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private const string SendMailBinding = "sendmail";
        private const string CreateBindingOperation = "create";

        private readonly DaprClient _daprClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(DaprClient daprClient, ILogger<EmailService> logger)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendOrderConfirmationAsync(Order order)
        {
            _logger.LogInformation("Sending order confirmation email for order {OrderId} to {BuyerEmail}.",
                order.Id, order.BuyerEmail);

            return _daprClient.InvokeBindingAsync(
                SendMailBinding,
                CreateBindingOperation,
                CreateEmailBody(order),
                new System.Collections.Generic.Dictionary<string, string>
                {
                    ["emailFrom"] = "eShopOn@dapr.io",
                    ["emailTo"] = order.BuyerEmail,
                    ["subject"] = $"Your eShopOnDapr Order #{order.OrderNumber}"
                });
        }

        private string CreateEmailBody(Order order)
        {
            var body = new StringBuilder();

            body.AppendLine($"Thank you for your order. The order number is: {order.OrderNumber}.");
            body.AppendLine();
            body.AppendLine("To follow the status of your order:");
            body.AppendLine();
            body.AppendLine("  1. log into the eShopOnDapr website,");
            body.AppendLine("  2. hover your mouse cursor over your account icon in the top-right corner,");
            body.AppendLine("  3. select 'My Orders' in the context-menu that appears,");
            body.AppendLine($"  4. click the 'Details' link for order with number {order.OrderNumber}.");
            body.AppendLine();
            body.AppendLine("Greetings,");
            body.AppendLine("The eShopOnDapr Team");

            return body.ToString();
        }
    }
}
