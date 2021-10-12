using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopOnDapr.BlazorClient.Ordering
{
    public class OrderForm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Card number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Cardholder name")]
        public string CardHolderName { get; set; }

        [Required]
        [CustomValidation(typeof(CardExpirationDate), "Validate")]
        [Display(Name = "Expiration date")]
        public string CardExpirationDate { get; set; }

        [Required]
        [Display(Name = "Security code")]
        public string CardSecurityCode { get; set; }
    }
}
