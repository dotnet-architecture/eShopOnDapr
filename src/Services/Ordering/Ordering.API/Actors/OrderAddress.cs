using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Actors
{
    public class OrderAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}