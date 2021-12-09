namespace Microsoft.eShopOnDapr.Services.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    public int ProductId { get; init; }
    public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; init; }
    public string PictureFileName { get; set; } = "";
    public bool IsVerified { get; set; } = false;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        return results;
    }
}
