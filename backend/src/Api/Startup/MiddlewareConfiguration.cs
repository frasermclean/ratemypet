using FastEndpoints;
using RateMyPet.ServiceDefaults;

namespace RateMyPet.Api.Startup;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseAuthorization();

        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Errors.UseProblemDetails();
        });

        app.MapHealthEndpoints();
        app.MapStaticAssets();
        app.MapFallbackToFile("index.html");

        return app;
    }
}
