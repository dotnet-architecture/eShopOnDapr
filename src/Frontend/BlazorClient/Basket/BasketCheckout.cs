using System;

namespace eShopOnDapr.BlazorClient.Ordering
{
    public class BasketCheckout
    {
        public string UserEmail { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpirationDate { get; set; }

        public string CardSecurityCode { get; set; }
    }
}
