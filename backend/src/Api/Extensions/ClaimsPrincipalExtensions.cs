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

    /// <summary>
    /// Gets the authenticated user ID from the claims principal or throws an exception if not found.
    /// </summary>
    /// <param name="principal">The claims principal to process</param>
    /// <returns>The authenticated user's ID</returns>
    /// <exception cref="InvalidOperationException">User ID was not found amongst the claims</exception>
    public static Guid GetAuthenticatedUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();

        if (userId is null)
        {
            throw new InvalidOperationException("User ID not found in claims principal.");
        }

        return userId.Value;
    }
}
