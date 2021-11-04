namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors;

public class OrderAddressState
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}