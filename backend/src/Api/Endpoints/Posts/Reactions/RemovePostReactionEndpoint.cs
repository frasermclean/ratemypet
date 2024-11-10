using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class RemovePostReactionEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}/reactions");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");
        var userId = User.GetUserId();
        var postReaction = await dbContext.PostReactions.FirstOrDefaultAsync(
            reaction => reaction.Post.Id == postId &&
                        reaction.User.Id == userId, cancellationToken);

        if (postReaction is null)
        {
            return TypedResults.NotFound();
        }

        dbContext.PostReactions.Remove(postReaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
