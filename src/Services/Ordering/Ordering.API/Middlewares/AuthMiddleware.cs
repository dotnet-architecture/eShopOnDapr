namespace Microsoft.eShopOnDapr.Services.Ordering.API.Middlewares;

public class AuthMiddleware : IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app)
    {
        app.UseAuthentication();
    }
}