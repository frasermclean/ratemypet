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

        app.UseFastEndpoints();

        // authentication endpoints
        app.MapGroup("auth")
            .MapIdentityApi<User>();

        return app;
    }
    
}