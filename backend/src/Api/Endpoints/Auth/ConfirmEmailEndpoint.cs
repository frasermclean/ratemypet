using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Auth;

public class ConfirmEmailEndpoint(UserManager<User> userManager)
    : Endpoint<ConfirmEmailRequest, Results<Ok, NotFound, ErrorResponse>>
{
    public override void Configure()
    {
        Post("auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, NotFound, ErrorResponse>> ExecuteAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded
            ? TypedResults.Ok()
            : new ErrorResponse(result.Errors.ToValidationFailures());
    }
}
