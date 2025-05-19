using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class VerifyUserEndpoint(UserManager<User> userManager)
    : EndpointWithoutRequest<Ok<VerifyUserResponse>>
{
    public override void Configure()
    {
        Get("/auth/verify-user");
        AllowAnonymous();
    }

    public override async Task<Ok<VerifyUserResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);

        return TypedResults.Ok(new VerifyUserResponse
        {
            IsAuthenticated = user is not null,
            User = user is not null
                ? new LoginResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    EmailAddress = user.Email!,
                    EmailHash = user.Email.ToSha256Hash(),
                    Roles = await userManager.GetRolesAsync(user)
                }
                : null
        });
    }
}
