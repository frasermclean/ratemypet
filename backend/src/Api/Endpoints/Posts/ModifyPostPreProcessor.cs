using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class ModifyPostPreProcessor : IPreProcessor<IModifyPostRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<IModifyPostRequest> context,
        CancellationToken cancellationToken)
    {
        if (context.Request is null)
        {
            return;
        }

        var dbContext = context.HttpContext.Resolve<ApplicationDbContext>();
        var logger = context.HttpContext.Resolve<ILogger<ModifyPostPreProcessor>>();
        var request = context.Request;

        var post = await dbContext.Posts.Where(post => post.Id == request.PostId)
            .Include(post => post.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            await context.HttpContext.Response.SendNotFoundAsync(cancellationToken);
            return;
        }

        var isAdministrator = context.HttpContext.User.IsInRole(Role.Administrator);
        var isOwner = post.User.Id == request.UserId;

        if (!isOwner && !isAdministrator)
        {
            logger.LogWarning("User with ID {UserId} attempted to modify post with ID {PostId} but is not authorized",
                request.UserId, request.PostId);

            await context.HttpContext.Response.SendForbiddenAsync(cancellationToken);
        }
    }
}
