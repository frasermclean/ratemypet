using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginEndpoint(SignInManager<User> signInManager, UserManager<User> userManager, TimeProvider timeProvider)
    : Endpoint<LoginRequest, Results<Ok<LoginResponse>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> ExecuteAsync(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var isEmailAddress = request.EmailOrUserName.IsEmailAddress();
        var user = isEmailAddress
            ? await userManager.FindByEmailAsync(request.EmailOrUserName)
            : await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user is null)
        {
            Logger.LogWarning("Could not find user with email or username {EmailOrUserName}", request.EmailOrUserName);
            return TypedResults.Unauthorized();
        }

        var result = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
        if (!result.Succeeded)
        {
            Logger.LogError("Failed to log in user {UserName}", user.UserName);
            return TypedResults.Unauthorized();
        }

        Logger.LogInformation("User {UserName} logged in", user.UserName);

        user.LastSeen = timeProvider.GetUtcNow().DateTime;
        await userManager.UpdateAsync(user);

        return TypedResults.Ok(new LoginResponse
        {
            UserId = user.Id,
            UserName = user.UserName!,
            EmailAddress = user.Email!,
            EmailHash = user.Email.ToSha256Hash(),
            Roles = await userManager.GetRolesAsync(user)
        });
    }
}
