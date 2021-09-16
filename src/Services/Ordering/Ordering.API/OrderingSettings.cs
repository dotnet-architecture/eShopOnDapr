namespace Microsoft.eShopOnDapr.Services.Ordering.API
{
    public class OrderingSettings
    {
        public int GracePeriodTime { get; set; }

        public bool SendConfirmationEmail { get; set; }
    }
}
