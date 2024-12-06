using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ResetPasswordEndpoint(UserManager<User> userManager)
    : Endpoint<ResetPasswordRequest, Results<Ok, ValidationProblem>>
{
    public override void Configure()
    {
        Post("auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ValidationProblem>> ExecuteAsync(ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.EmailAddress);

        if (user is null || await userManager.IsEmailConfirmedAsync(user) == false)
        {
            // don't reveal if the user does not exist or is not confirmed
            return CreateValidationProblem();
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
        }
        catch (FormatException exception)
        {
            Logger.LogError(exception, "Error decoding reset code");
            return CreateValidationProblem();
        }

        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            return CreateValidationProblem();
        }

        return TypedResults.Ok();
    }

    private static ValidationProblem CreateValidationProblem()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "ResetCode", ["Invalid reset code"] }
        };
        return TypedResults.ValidationProblem(errors);
    }
}
