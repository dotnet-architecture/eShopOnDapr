using System;

namespace Microsoft.eShopOnDapr.BlazorClient.Ordering
{
    public record BasketCheckout(
        string UserEmail,
        string Street,
        string City,
        string State,
        string Country,
        string CardNumber,
        string CardHolderName,
        DateTime CardExpirationDate,
        string CardSecurityCode);
}
