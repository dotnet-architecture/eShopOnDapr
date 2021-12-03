namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.Services;

public class EmailService : IEmailService
{
    private const string SendMailBinding = "sendmail";
    private const string CreateBindingOperation = "create";

    private readonly DaprClient _daprClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(DaprClient daprClient, ILogger<EmailService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public Task SendOrderConfirmationAsync(Order order)
    {
        _logger.LogInformation("Sending order confirmation email for order {OrderId} to {BuyerEmail}.",
            order.Id, order.BuyerEmail);

        var message = CreateEmailBody(order);

        return _daprClient.InvokeBindingAsync(
            SendMailBinding,
            CreateBindingOperation,
            message,
            new Dictionary<string, string>
            {
                ["emailFrom"] = "eshopondapr@example.com",
                ["emailTo"] = order.BuyerEmail,
                ["subject"] = $"Your eShopOnDapr Order #{order.OrderNumber}"
            });
    }

    private static string CreateEmailBody(Order order) =>
        $@"
            <html>
            <body>
                <h1>Your order confirmation</h1>
                <p>Thank you for your order! The order number is {order.OrderNumber}.</p>
                <p>To follow the status of your order:</p>
                <ol>
                	<li>Log onto the eShopOnDapr website</li>
                    <li>Hover your mouse cursor over your account icon in the top-right corner</li>
                    <li>Select 'My Orders' in the context-menu that appears</li>
                    <li>Click the 'Details' link for order with number {order.OrderNumber}</li>
                </ol>
                <p>Greetings,</p>
                <p>The eShopOnDapr Team</p>
            </body>
            </html>";
}
