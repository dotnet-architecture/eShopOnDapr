namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }

    public Address() : this(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty)
    {
    }

    public Address(
        string street,
        string city,
        string state,
        string country)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
    }
}
