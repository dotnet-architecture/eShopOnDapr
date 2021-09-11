using System;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors
{
    public class OrderAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}