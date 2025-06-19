using System.Diagnostics;
using FastEndpoints;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class ModifyPostPreProcessor : IPreProcessor<IModifyPostRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<IModifyPostRequest> context,
        CancellationToken cancellationToken)
    {
        var dbContext = context.HttpContext.Resolve<ApplicationDbContext>();
        var logger = context.HttpContext.Resolve<ILogger<ModifyPostPreProcessor>>();
        var request = context.Request;

        Debug.Assert(request is not null);

        var post = await dbContext.Posts.FindAsync([request.PostId], cancellationToken);
        if (post is null)
        {
            await context.HttpContext.Response.SendNotFoundAsync(cancellationToken);
            return;
        }

        var isAdministrator = context.HttpContext.User.IsInRole(Role.Administrator);
        var isOwner = post.UserId == request.UserId;

        if (!isOwner && !isAdministrator)
        {
            logger.LogWarning("User with ID {UserId} attempted to modify post with ID {PostId} but is not authorized",
                request.UserId, request.PostId);

            await context.HttpContext.Response.SendForbiddenAsync(cancellationToken);
        }
    }
}
