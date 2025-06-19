using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class RemovePostReactionEndpoint(ApplicationDbContext dbContext)
    : Endpoint<RemovePostReactionRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}/reactions");
        Roles(Role.Contributor);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(
        RemovePostReactionRequest request, CancellationToken cancellationToken)
    {
        var postReaction = await dbContext.PostReactions.FirstOrDefaultAsync(
            reaction => reaction.UserId == request.UserId && reaction.PostId == request.PostId, cancellationToken);

        if (postReaction is null)
        {
            return TypedResults.NotFound();
        }

        dbContext.Remove(postReaction);
        dbContext.Add(PostUserActivity.DeleteReaction(request.UserId, request.PostId));
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Removed reaction for user with ID {UserId} on post with ID {PostId}",
            request.UserId, request.PostId);

        return TypedResults.NoContent();
    }
}
