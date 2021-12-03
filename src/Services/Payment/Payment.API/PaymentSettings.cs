namespace Microsoft.eShopOnDapr.Services.Payment.API;

public class PaymentSettings
{
    public bool PaymentSucceeded { get; set; } = true;

    public decimal? MaxOrderTotal { get; set; } = null!;
}
