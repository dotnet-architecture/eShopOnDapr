namespace Microsoft.eShopOnDapr.Services.Basket.API.Actors;

public class BasketStatus
{
    public static readonly BasketStatus New                         = new(0, nameof(New));
    public static readonly BasketStatus AwaitingProductValidation   = new(2, nameof(AwaitingProductValidation));
    public static readonly BasketStatus Validated                   = new(3, nameof(Validated));

    public int Id { get; set; }

    public string Name { get; set; }

    public BasketStatus()
        : this(New.Id, New.Name)
    {
    }

    public BasketStatus(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
