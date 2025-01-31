using System.Text;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ConfirmEmailEndpoint(UserManager<User> userManager)
    : Endpoint<ConfirmEmailRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        foreach (var error in result.Errors)
        {
            AddError(new ValidationFailure(error.Code, error.Description));
        }

        ThrowIfAnyErrors();

        // assign default roles
        await userManager.AddToRoleAsync(user, Role.Contributor);

        return TypedResults.NoContent();
    }
}
