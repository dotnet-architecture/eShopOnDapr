namespace Microsoft.eShopOnDapr.Services.Ordering.API.Middlewares;

public interface IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app);
}