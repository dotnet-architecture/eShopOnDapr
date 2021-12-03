namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record UserCheckoutAcceptedIntegrationEvent(
    string UserId,
    string UserEmail,
    string City,
    string Street,
    string State,
    string Country,
    string CardNumber,
    string CardHolderName,
    DateTime CardExpiration,
    string CardSecurityNumber,
    Guid RequestId,
    CustomerBasket Basket)
    : IntegrationEvent;
