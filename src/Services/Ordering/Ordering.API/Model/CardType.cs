using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class CardType
    {
        public static readonly CardType Amex = new CardType(1, "Amex");
        public static readonly CardType Visa = new CardType(2, "Visa");
        public static readonly CardType MasterCard = new CardType(3, "MasterCard");

        public CardType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
