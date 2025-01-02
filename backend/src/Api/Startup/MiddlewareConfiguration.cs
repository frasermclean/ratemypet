using FastEndpoints;
using RateMyPet.Core.Security;
using SixLabors.ImageSharp.Web.DependencyInjection;

namespace RateMyPet.Api.Startup;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors();
        }

        app.UseImageSharp();

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
