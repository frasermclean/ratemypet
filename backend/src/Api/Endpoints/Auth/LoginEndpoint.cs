using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

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
        var isEmailAddress = request.EmailOrUserName.IsEmailAddress();
        var user = isEmailAddress
            ? await userManager.FindByEmailAsync(request.EmailOrUserName)
            : await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user is null)
        {
            Logger.LogWarning("Could not find user with email or username {EmailOrUserName}", request.EmailOrUserName);
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
        if (!result.Succeeded)
        {
            Logger.LogError("Failed to log in user {UserName}", user.UserName);
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        user.LastSeen = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        Logger.LogInformation("User {UserName} logged in", user.UserName);
    }
}
