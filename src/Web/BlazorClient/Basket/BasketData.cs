namespace Microsoft.eShopOnDapr.BlazorClient.Basket;

public record BasketData(IEnumerable<BasketItem> Items, bool IsVerified = false);
