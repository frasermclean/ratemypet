using System.Security.Claims;

namespace RateMyPet.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    public static string? GetUserName(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Name);

    public static string? GetEmailAddress(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Email);

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal) =>
        principal.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value);
}
