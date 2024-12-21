using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class VerifyUserEndpoint(UserManager<User> userManager)
    : EndpointWithoutRequest<Results<Ok<LoginResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/auth/verify-user");
    }

    public override async Task<Results<Ok<LoginResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        return user is not null
            ? TypedResults.Ok(new LoginResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                EmailAddress = user.Email!,
                EmailHash = user.Email.ToSha256Hash(),
                Roles = await userManager.GetRolesAsync(user)
            })
            : TypedResults.NotFound();
    }
}
