using FastEndpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginEndpoint(SignInManager<User> signInManager, UserManager<User> userManager)
    : Endpoint<LoginRequest>
{
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var isEmailAddress = request.EmailOrUserName.Contains('@');
        var user = isEmailAddress
            ? await userManager.FindByEmailAsync(request.EmailOrUserName)
            : await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user is null)
        {
            Logger.LogWarning("Could not find user with email or username {EmailOrUserName}", request.EmailOrUserName);
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
        if (!result.Succeeded)
        {
            Logger.LogError("Failed to log in user {UserName}", user.UserName);
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        Logger.LogInformation("User {UserName} logged in", user.UserName);
    }
}
