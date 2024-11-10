using System.Security.Claims;

namespace RateMyPet.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);

        return Guid.Parse(claim.Value);
    }
}
