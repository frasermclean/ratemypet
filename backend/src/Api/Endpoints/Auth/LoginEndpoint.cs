using FastEndpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginEndpoint(SignInManager<User> signInManager)
    : Endpoint<LoginRequest, Results<Ok<AccessTokenResponse>, EmptyHttpResult, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, UnauthorizedHttpResult>> ExecuteAsync(
        LoginRequest request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = request.UseCookies
            ? IdentityConstants.ApplicationScheme
            : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);

        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        Logger.LogInformation("User {UserName} logged in", request.UserName);
        return TypedResults.Empty;
    }
}
