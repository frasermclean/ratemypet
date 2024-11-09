using FastEndpoints;
using Microsoft.AspNetCore.Identity;
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

        // authentication endpoints provided by Identity
        var authGroup = app.MapGroup("auth");
        authGroup.MapIdentityApi<User>();

        // logout endpoint
        authGroup.MapPost("/logout", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.NoContent();
        }).RequireAuthorization();

        return app;
    }
    
}