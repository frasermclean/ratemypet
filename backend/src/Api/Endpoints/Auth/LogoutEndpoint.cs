using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class LogoutEndpoint(SignInManager<User> signInManager) : EndpointWithoutRequest<NoContent>
{
    public override void Configure()
    {
        Post("auth/logout");
    }

    public override async Task<NoContent> ExecuteAsync(CancellationToken ct)
    {
        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        await signInManager.SignOutAsync();

        Logger.LogInformation("User {UserName} logged out", User.Identity!.Name);

        return TypedResults.NoContent();
    }
}
