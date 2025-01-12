using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class VerifyUserEndpoint(UserManager<User> userManager)
    : EndpointWithoutRequest<Ok<LoginResponse>>
{
    public override void Configure()
    {
        Get("/auth/verify-user");
    }

    public override async Task<Ok<LoginResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);

        Debug.Assert(user is not null, "Authenticated user should never be null");

        return TypedResults.Ok(new LoginResponse
        {
            UserId = user.Id,
            UserName = user.UserName!,
            EmailAddress = user.Email!,
            EmailHash = user.Email.ToSha256Hash(),
            Roles = await userManager.GetRolesAsync(user)
        });
    }
}
