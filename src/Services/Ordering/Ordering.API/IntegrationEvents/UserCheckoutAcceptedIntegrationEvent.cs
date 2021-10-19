using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnDapr.Services.Ordering.API.Application.Models;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents
{
    public class UserCheckoutAcceptedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        public Guid RequestId { get; set; }

        public CustomerBasket Basket { get; }

        public UserCheckoutAcceptedIntegrationEvent(
            string userId,
            string userEmail,
            string city,
            string street,
            string state,
            string country,
            string cardNumber,
            string cardHolderName,
            DateTime cardExpiration,
            string cardSecurityNumber,
            Guid requestId,
            CustomerBasket basket)
        {
            UserId = userId;
            UserEmail = userEmail;
            City = city;
            Street = street;
            State = state;
            Country = country;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            Basket = basket;
            RequestId = requestId;
        }
    }
}
