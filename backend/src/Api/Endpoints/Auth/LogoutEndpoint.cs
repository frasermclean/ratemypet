using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class LogoutEndpoint(SignInManager<User> signInManager) : EndpointWithoutRequest<NoContent>
{
    public override void Configure()
    {
        Post("auth/logout");
    }

    public override async Task<NoContent> ExecuteAsync(CancellationToken ct)
    {
        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }
}
