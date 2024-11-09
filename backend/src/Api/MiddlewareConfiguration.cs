using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseFastEndpoints();

        // authentication endpoints
        app.MapGroup("auth")
            .MapIdentityApi<User>();

        return app;
    }
    
}