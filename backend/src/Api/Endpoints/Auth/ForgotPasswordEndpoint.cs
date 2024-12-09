using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Services;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ForgotPasswordEndpoint(UserManager<User> userManager, IEmailSender emailSender)
    : Endpoint<ForgotPasswordRequest>
{
    public override void Configure()
    {
        Post("auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is null)
        {
            Logger.LogWarning("Could not find user with email address: {EmailAddress}", request.EmailAddress);
            await CreateRandomDelayAsync(cancellationToken); // delay to prevent timing attacks
            return;
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            Logger.LogWarning("User {UserName} has not confirmed their email address", user.UserName);
            await CreateRandomDelayAsync(cancellationToken); // delay to prevent timing attacks
            return;
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await emailSender.SendPasswordResetLinkAsync(request.EmailAddress, token);
            Logger.LogInformation("Sent password reset link for user {UserName}", user.UserName);
        }
    }

    private static Task CreateRandomDelayAsync(CancellationToken cancellationToken) =>
        Task.Delay(Random.Shared.Next(2500, 8000), cancellationToken);
}
