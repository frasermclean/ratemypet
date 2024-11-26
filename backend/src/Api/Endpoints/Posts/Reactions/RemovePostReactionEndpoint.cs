using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Persistence.Services;

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

        var deletedReactionCount = await dbContext.PostReactions
            .Where(reaction => reaction.User.Id == userId && reaction.Post.Id == postId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedReactionCount > 0)
        {
            Logger.LogInformation("Removed reaction from post {PostId} for user with ID: {UserId}", postId, userId);
        }

        return TypedResults.NoContent();
    }
}
