using RateMyPet.Core;
using RateMyPet.Core.Security;

namespace RateMyPet.Infrastructure.Extensions;

public static class RoleDefinitionExtensions
{
    public static Role ToRole(this RoleDefinition roleDefinition) => new()
    {
        Id = roleDefinition.Id,
        Name = roleDefinition.Name,
        NormalizedName = roleDefinition.Name.ToUpperInvariant()
    };
}
