using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Auth;

public class LogoutEndpoint(SignInManager<User> signInManager) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("auth/logout");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();

        Logger.LogInformation("User {UserName} logged out", User.Identity!.Name);
    }
}
