namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(Order order);
}
