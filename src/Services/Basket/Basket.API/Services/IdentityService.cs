namespace Microsoft.eShopOnDapr.Services.Basket.API.Services;

public class IdentityService : IIdentityService
{
    private IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string GetUserIdentity()
    {
        return _context.HttpContext?.User.FindFirst("sub")?.Value ?? "";
    }
}
