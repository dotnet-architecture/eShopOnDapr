using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class CardTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static CardTypeDto FromCardType(CardType cardType)
        {
            return new CardTypeDto
            {
                Id = cardType.Id,
                Name = cardType.Name
            };
        }
    }
}
