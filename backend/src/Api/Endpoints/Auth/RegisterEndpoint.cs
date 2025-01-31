using System.Text;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;

namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterEndpoint(
    UserManager<User> userManager,
    IMessagePublisher messagePublisher)
    : Endpoint<RegisterRequest, NoContent>
{
    public override void Configure()
    {
        Post("auth/register");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.EmailAddress
        };

        var result = await userManager.CreateAsync(user, request.Password);

        foreach (var error in result.Errors)
        {
            AddError(new ValidationFailure(error.Code, error.Description));
        }

        ThrowIfAnyErrors();

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await messagePublisher.PublishAsync(new RegisterConfirmationMessage
        {
            EmailAddress = user.Email,
            UserId = user.Id,
            ConfirmationToken = confirmationToken
        }, cancellationToken);

        return TypedResults.NoContent();
    }
}
