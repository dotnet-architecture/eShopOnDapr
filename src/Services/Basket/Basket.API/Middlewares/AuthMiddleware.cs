namespace Microsoft.eShopOnDapr.Services.Basket.API.Middlewares;

public class AuthMiddleware : IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app)
    {
        app.UseAuthentication();
    }
}