using FastEndpoints;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Startup;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors();
        }

        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Errors.UseProblemDetails();
            config.Errors.ProducesMetadataType = typeof(ProblemDetails);
        });

        // authentication endpoints provided by Identity
        var authGroup = app.MapGroup("auth");
        authGroup.MapIdentityApi<User>();

        return app;
    }
}
