using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class RemovePostReactionEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<Ok<PostReactionsResponse>, NotFound, BadRequest>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}/reactions");
    }

    public override async Task<Results<Ok<PostReactionsResponse>, NotFound, BadRequest>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");
        var userId = User.GetUserId();

        var deletedReactionCount = await dbContext.PostReactions
            .Where(reaction => reaction.User.Id == userId && reaction.Post.Id == postId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedReactionCount == 0)
        {
            return TypedResults.BadRequest();
        }

        var response = await dbContext.Posts
            .AsNoTracking()
            .Where(post => post.Id == postId)
            .Select(post => new PostReactionsResponse
            {
                LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.NotFound();
    }
}
