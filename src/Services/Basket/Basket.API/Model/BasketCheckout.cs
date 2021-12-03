namespace Microsoft.eShopOnDapr.Services.Basket.API.Model;

public record BasketCheckout(
    string UserEmail,
    string City,
    string Street,
    string State,
    string Country,
    string CardNumber,
    string CardHolderName,
    DateTime CardExpiration,
    string CardSecurityCode
);


