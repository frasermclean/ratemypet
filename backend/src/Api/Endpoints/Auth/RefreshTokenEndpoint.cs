using FastEndpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class RefreshTokenEndpoint(
    SignInManager<User> signInManager,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
    TimeProvider timeProvider)
    : Endpoint<RefreshTokenRequest, Results<Ok<AccessTokenResponse>, ChallengeHttpResult, SignInHttpResult>>
{
    public override void Configure()
    {
        Post("auth/refresh-token");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<AccessTokenResponse>, ChallengeHttpResult, SignInHttpResult>> ExecuteAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var protector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = protector.Unprotect(request.RefreshToken);

        // reject the refresh attempt if the token is expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
        {
            return TypedResults.Challenge();
        }

        var principal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme);
    }
}
