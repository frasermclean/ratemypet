using System.Text;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterUserEndpoint(
    UserManager<User> userManager,
    IEmailSender<User> emailSender,
    IOptions<FrontendOptions> frontendOptions)
    : Endpoint<RegisterUserRequest, Results<Ok, ErrorResponse>>
{
    private readonly string frontendBaseUrl = frontendOptions.Value.BaseUrl;

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ErrorResponse>> ExecuteAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.EmailAddress
        };
        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var failures = createResult.Errors.Select(error => new ValidationFailure(error.Code, error.Description))
                .ToList();
            return new ErrorResponse(failures);
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
