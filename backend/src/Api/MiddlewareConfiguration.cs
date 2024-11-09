using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api;

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