using System.Security.Claims;

namespace RateMyPet.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        return claim is not null
            ? Guid.Parse(claim.Value)
            : null;
    }
}
