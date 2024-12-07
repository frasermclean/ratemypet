using FastEndpoints;
using RateMyPet.Core.Security;

namespace RateMyPet.Api.Startup;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors();
        }

        app.UseAuthorization();

        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Errors.UseProblemDetails();
            config.Errors.ProducesMetadataType = typeof(ProblemDetails);
            config.Security.PermissionsClaimType = Claims.Permissions;
        });

        return app;
    }
}
