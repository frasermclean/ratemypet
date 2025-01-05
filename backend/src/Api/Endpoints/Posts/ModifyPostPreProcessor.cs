using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Infrastructure.Services;

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

        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId && p.User.Id == request.UserId,
            cancellationToken);

        if (post is null)
        {
            logger.LogWarning("User with ID {UserId} attempted to modify post with ID {PostId} but is not authorized",
                request.UserId, request.PostId);

            await context.HttpContext.Response.SendForbiddenAsync(cancellationToken);
        }
    }
}
