extern alias basket;

using basket.Microsoft.eShopOnDapr.Services.Basket.API.Middlewares;

namespace FunctionalTests.Services.Basket;

internal class TestAuthMiddleware : IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app)
    {
        app.UseMiddleware<AutoAuthorizeMiddleware>();
    }
}