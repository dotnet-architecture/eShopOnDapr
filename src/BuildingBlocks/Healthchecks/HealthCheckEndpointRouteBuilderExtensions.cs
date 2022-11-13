using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class HealthCheckEndpointRouteBuilderExtensions
{
    public static void MapCustomHealthChecks(
        this WebApplication app,
        string healthPattern = "/hc",
        string livenessPattern = "/liveness",
        Func<AspNetCore.Http.HttpContext, HealthReport, Task> responseWriter = default)
    {
        app.MapHealthChecks(healthPattern, new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = responseWriter,
        });
        app.MapHealthChecks(livenessPattern, new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }
}
