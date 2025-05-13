using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace RateMyPet.ServiceDefaults;

public static class WebApplicationExtensions
{
    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(EndpointPaths.Health);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(EndpointPaths.Aliveness, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
