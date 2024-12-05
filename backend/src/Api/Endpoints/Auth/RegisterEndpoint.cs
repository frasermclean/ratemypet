using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Extensions;
using RateMyPet.Api.Options;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterEndpoint(
    UserManager<User> userManager,
    IEmailSender<User> emailSender,
    IOptions<FrontendOptions> frontendOptions)
    : Endpoint<RegisterRequest, Results<Ok, ValidationProblem>>
{
    private readonly string frontendBaseUrl = frontendOptions.Value.BaseUrl;

    public override void Configure()
    {
        Post("auth/register");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ValidationProblem>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.EmailAddress
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return TypedResults.ValidationProblem(result.Errors.ToDictionary());
        }

        await SendConfirmationLinkAsync(user, request.EmailAddress);

        return TypedResults.Ok();
    }

    private async Task SendConfirmationLinkAsync(User user, string emailAddress)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationLink = $"{frontendBaseUrl}/auth/confirm-email?userId={user.Id}&token={confirmationToken}";

        await emailSender.SendConfirmationLinkAsync(user, emailAddress, confirmationLink);
    }
}
