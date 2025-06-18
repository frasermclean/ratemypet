using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class UpdatePostReactionEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostReactionRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}/reactions");
        Roles(Role.Contributor);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(
        UpdatePostReactionRequest request, CancellationToken cancellationToken)
    {
        var postReaction = await dbContext.PostReactions.FirstOrDefaultAsync(
            reaction => reaction.UserId == request.UserId && reaction.PostId == request.PostId, cancellationToken);

        if (postReaction is null)
        {
            return TypedResults.NotFound();
        }

        postReaction.Reaction = request.Reaction;
        dbContext.Add(PostUserActivity.UpdateReaction(request.UserId, request.PostId, request.Reaction));

        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Updated reaction to {Reaction} for user with ID {UserId} on post with ID {PostId}",
            request.Reaction, request.UserId, request.PostId);

        return TypedResults.NoContent();
    }
}
