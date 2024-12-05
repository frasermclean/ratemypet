using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Security;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Services;

public class SecurityInitializer(IServiceScopeFactory serviceScopeFactory, ILogger<SecurityInitializer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await EnsureRoleExistsAsync(Roles.User);
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var userRole = await roleManager.FindByNameAsync(roleName);
        if (userRole is null)
        {
            userRole = new Role { Name = roleName };
            await roleManager.CreateAsync(userRole);
            logger.LogInformation("Created role {RoleName}", roleName);
        }

        var existingClaims = await roleManager.GetClaimsAsync(userRole);

        var claimsToAdd = Roles.PermissionsMap[Roles.User].Except(existingClaims.Select(claim => claim.Value))
            .Select(permission => new Claim(Claims.Permissions, permission));

        foreach (var claim in claimsToAdd)
        {
            logger.LogInformation("Adding claim {ClaimType} with value {ClaimValue} to role {RoleName}",
                claim.Type, claim.Value, roleName);
            await roleManager.AddClaimAsync(userRole, claim);
        }
    }
}
