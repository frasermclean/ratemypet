using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Users;

public class GetCurrentUserEndpoint(UserManager<User> userManager)
    : EndpointWithoutRequest<Results<Ok<UserResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/users/me");
    }

    public override async Task<Results<Ok<UserResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);

        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var response = new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            EmailAddress = user.Email!,
            EmailHash = user.Email.ToSha256Hash()
        };

        return TypedResults.Ok(response);
    }
}
