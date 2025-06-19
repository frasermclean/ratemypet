using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Api.Extensions;

namespace RateMyPet.Api.Endpoints.Auth;

public class VerifyUserEndpoint : EndpointWithoutRequest<Ok<VerifyUserResponse>>
{
    public override void Configure()
    {
        Get("/auth/verify-user");
        AllowAnonymous();
    }

    public override Task<Ok<VerifyUserResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userName = User.GetUserName();
        var emailAddress = User.GetEmailAddress();

        var response = new VerifyUserResponse
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            User = userId is not null && userName is not null && emailAddress is not null
                ? new LoginResponse
                {
                    UserId = userId.Value,
                    UserName = userName,
                    EmailAddress = emailAddress,
                    EmailHash = emailAddress.ToSha256Hash(),
                    Roles = User.GetRoles(),
                }
                : null
        };

        return Task.FromResult(TypedResults.Ok(response));
    }
}
