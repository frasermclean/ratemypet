using System.Text;
using System.Text.Encodings.Web;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Api.Extensions;
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
        var isEmailAddress = request.EmailOrUserName.IsEmailAddress();

        var user = isEmailAddress
            ? await userManager.FindByEmailAsync(request.EmailOrUserName)
            : await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user is null)
        {
            Logger.LogWarning("Could not find user with email or username {EmailOrUserName}", request.EmailOrUserName);
            await Task.Delay(Random.Shared.Next(2500, 8000), cancellationToken); // delay to prevent timing attacks
            return;
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetCode = HtmlEncoder.Default.Encode(WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)));

            await emailSender.SendPasswordResetCodeAsync(user.Email!, resetCode);

            Logger.LogInformation("Sent password reset code for user {UserName}", user.UserName);
        }
    }
}
