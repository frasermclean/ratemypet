using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Auth;

public class LogoutEndpoint(SignInManager<User> signInManager, ApplicationDbContext dbContext) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("auth/logout");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();

        dbContext.UserActivities.Add(new UserActivity((Guid)User.GetUserId()!, Activity.Logout));
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("User {UserName} logged out", User.Identity!.Name);
    }
}
