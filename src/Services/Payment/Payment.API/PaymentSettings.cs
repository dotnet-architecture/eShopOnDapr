namespace Microsoft.eShopOnDapr.Services.Payment.API
{
    public class PaymentSettings
    {
        public bool PaymentSucceeded { get; set; }
        public decimal? MaxOrderTotal { get; set; }
    }
}
