using FastEndpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginEndpoint(SignInManager<User> signInManager, UserManager<User> userManager)
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
        var user = await userManager.FindByEmailAsync(request.EmailOrPassword)
                   ?? await userManager.FindByNameAsync(request.EmailOrPassword);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        signInManager.AuthenticationScheme = request.UseCookies
            ? IdentityConstants.ApplicationScheme
            : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        Logger.LogInformation("User {EmailOrPassword} logged in", request.EmailOrPassword);
        return TypedResults.Empty;
    }
}
