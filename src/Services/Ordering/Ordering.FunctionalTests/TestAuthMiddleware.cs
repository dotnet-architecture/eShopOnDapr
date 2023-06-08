namespace Ordering.FunctionalTests;

internal class TestAuthMiddleware : IAuthMiddleware
{
    public void UseAuth(IApplicationBuilder app)
    {
        app.UseMiddleware<AutoAuthorizeMiddleware>();
    }
}