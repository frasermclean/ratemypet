using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Roles;

public class DeleteRoleEndpoint(RoleManager<Role> roleManager)
    : EndpointWithoutRequest<Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Delete("roles/{roleId:guid}");
        // Roles("Administrator");
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var roleId = Route<Guid>("roleId");

        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
        {
            return TypedResults.NotFound();
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded
            ? TypedResults.NoContent()
            : TypedResults.ValidationProblem(result.Errors.ToDictionary());
    }
}
