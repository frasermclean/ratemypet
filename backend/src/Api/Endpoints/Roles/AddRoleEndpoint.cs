using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence.Models;
using SecurityClaims = RateMyPet.Api.Security.Claims;

namespace RateMyPet.Api.Endpoints.Roles;

public class AddRoleEndpoint(RoleManager<Role> roleManager, UserManager<User> userManager)
    : Endpoint<AddRoleRequest, Results<Created, ValidationProblem>>
{
    public override void Configure()
    {
        Post("roles");
        // Roles("Administrator");
    }

    public override async Task<Results<Created, ValidationProblem>> ExecuteAsync(AddRoleRequest request,
        CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Name = request.Name,
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return TypedResults.ValidationProblem(result.Errors.ToDictionary());
        }

        var claims = request.Permissions.Select(p => new Claim(SecurityClaims.Permission, p));
        foreach (var claim in claims)
        {
            await roleManager.AddClaimAsync(role, claim);
        }

        if (request.AddCurrentUser)
        {
            var user = await userManager.GetUserAsync(User);
            await userManager.AddToRoleAsync(user!, role.Name);
        }

        return TypedResults.Created($"/roles/{role.Id}");
    }
}
