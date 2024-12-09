using System.Text;
using System.Text.Encodings.Web;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;
using RateMyPet.Api.Services;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ForgotPasswordEndpoint(
    UserManager<User> userManager,
    IEmailSender emailSender,
    IOptions<FrontendOptions> frontendOptions)
    : Endpoint<ForgotPasswordRequest>
{
    private readonly string frontendBaseUrl = frontendOptions.Value.BaseUrl;

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
            var resetCode = HtmlEncoder.Default.Encode(WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)));
            var resetLink = $"{frontendBaseUrl}/auth/reset-password?emailAddress={user.Email}&resetCode={resetCode}";

            await emailSender.SendPasswordResetLinkAsync(request.EmailAddress, resetLink);
            Logger.LogInformation("Sent password reset link for user {UserName}", user.UserName);
        }
    }

    private static Task CreateRandomDelayAsync(CancellationToken cancellationToken) =>
        Task.Delay(Random.Shared.Next(2500, 8000), cancellationToken);
}
