using System;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model
{
    public class OrderSummary
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal Total { get; set; }
    }
}
