namespace Payment.API
{
    public class PaymentSettings
    {
        public bool PaymentSucceeded { get; set; }
        public decimal? MaxOrderTotal { get; set; }
        public string EventBusConnection { get; set; }
    }
}
