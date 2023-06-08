extern alias ordering;

using ordering.Microsoft.eShopOnDapr.Services.Ordering.API.Middlewares;

namespace FunctionalTests.Services.Ordering;

internal class TestAuthMiddleware : IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app)
    {
        app.UseMiddleware<AutoAuthorizeMiddleware>();
    }
}