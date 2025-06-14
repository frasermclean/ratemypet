using System.Text;
using System.Text.Encodings.Web;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Api.Endpoints.Auth;

public class ForgotPasswordEndpoint(UserManager<User> userManager, IMessagePublisher messagePublisher)
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

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetCode = HtmlEncoder.Default.Encode(WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)));

        await messagePublisher.PublishAsync(new ForgottenPasswordMessage
        {
            UserId = user.Id,
            EmailAddress = request.EmailAddress,
            ResetCode = resetCode
        }, cancellationToken);

        Logger.LogInformation("Published forgotten password message for {UserName}", user.UserName);
    }

    private static Task CreateRandomDelayAsync(CancellationToken cancellationToken) =>
        Task.Delay(Random.Shared.Next(250, 1000), cancellationToken);
}
