using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class GetCurrentUserEndpoint(UserManager<User> userManager)
    : EndpointWithoutRequest<Results<Ok<GetCurrentUserResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/auth/current-user");
    }

    public override async Task<Results<Ok<GetCurrentUserResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var response = new GetCurrentUserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            EmailAddress = user.Email!,
            EmailHash = user.Email.ToSha256Hash(),
            Roles = await userManager.GetRolesAsync(user)
        };

        return TypedResults.Ok(response);
    }
}
