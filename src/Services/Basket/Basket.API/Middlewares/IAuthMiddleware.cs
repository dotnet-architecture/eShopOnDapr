namespace Microsoft.eShopOnDapr.Services.Basket.API.Middlewares;

public interface IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app);
}