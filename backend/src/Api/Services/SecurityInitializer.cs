using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Core;
using RateMyPet.Core.Security;

namespace RateMyPet.Api.Services;

public class SecurityInitializer(IServiceScopeFactory serviceScopeFactory, ILogger<SecurityInitializer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await EnsureRoleExistsAsync(Roles.User);
    }

    private async Task EnsureRoleExistsAsync(RoleDefinition roleDefinition)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var userRole = await roleManager.FindByNameAsync(roleDefinition.Name);
        if (userRole is null)
        {
            userRole = new Role { Name = roleDefinition.Name };
            await roleManager.CreateAsync(userRole);
            logger.LogInformation("Created role {RoleName}", roleDefinition.Name);
        }

        var existingClaims = await roleManager.GetClaimsAsync(userRole);

        var claimsToAdd = Roles.PermissionsMap[roleDefinition].Except(existingClaims.Select(claim => claim.Value))
            .Select(permission => new Claim(Claims.Permissions, permission));

        foreach (var claim in claimsToAdd)
        {
            logger.LogInformation("Adding claim {ClaimType} with value {ClaimValue} to role {RoleName}",
                claim.Type, claim.Value, roleDefinition.Name);
            await roleManager.AddClaimAsync(userRole, claim);
        }
    }
}
