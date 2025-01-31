using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ResetPasswordEndpoint(UserManager<User> userManager)
    : Endpoint<ResetPasswordRequest>
{
    public override void Configure()
    {
        Post("auth/reset-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var token = "";
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
        }
        catch (FormatException exception)
        {
            Logger.LogError(exception, "Error decoding reset code");
            ThrowError(r => r.ResetCode, "Invalid reset code");
        }

        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is null || await userManager.IsEmailConfirmedAsync(user) == false)
        {
            // don't reveal if the user does not exist or is not confirmed
            ThrowError(r => r.ResetCode, "Invalid reset code");
        }

        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            ThrowError(r => r.ResetCode, "Invalid reset code");
        }
    }
}
