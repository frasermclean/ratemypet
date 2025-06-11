using FastEndpoints;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Users;

public class SearchUsersEndpoint(UserManager<User> userManager) : Endpoint<GridifyQuery, Paging<SearchUsersMatch>>
{
    public override void Configure()
    {
        Get("users");
        Roles(Role.Administrator);
    }

    public override async Task<Paging<SearchUsersMatch>> ExecuteAsync(GridifyQuery query,
        CancellationToken cancellationToken)
    {
        var paging = await userManager.Users
            .Select(user => new SearchUsersMatch
            {
                Id = user.Id,
                UserName = user.UserName!,
                EmailAddress = user.Email!,
                IsEmailConfirmed = user.EmailConfirmed,
                LastSeen = user.LastActivity,
                Roles = user.Roles.Select(role => role.Name!)
            })
            .GridifyAsync(query, cancellationToken);

        return paging;
    }
}
