using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Api.Extensions;

public static class IdentityErrorsExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this IEnumerable<IdentityError> errors) =>
        errors.GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
}
