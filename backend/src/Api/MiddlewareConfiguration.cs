using FastEndpoints;

namespace RateMyPet.Api;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
        });

        return app;
    }
    
}