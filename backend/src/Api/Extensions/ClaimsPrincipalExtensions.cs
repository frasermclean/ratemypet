using System.Security.Claims;

namespace RateMyPet.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Attempts to get the user id from the claims principal.
    /// </summary>
    /// <param name="principal">The claims principal to process</param>
    /// <returns>The user ID if found, null if not</returns>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
