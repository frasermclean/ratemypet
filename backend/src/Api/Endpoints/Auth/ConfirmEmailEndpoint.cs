using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

public class ConfirmEmailEndpoint(UserManager<User> userManager)
    : Endpoint<ConfirmEmailRequest, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("auth/confirm-email");
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(
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
        if (!result.Succeeded)
        {
            return result.ToProblemDetails();
        }

        // assign default roles
        await userManager.AddToRoleAsync(user, Role.Contributor);

        return TypedResults.NoContent();
    }
}
