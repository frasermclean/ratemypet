using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class UpdatePostReactionEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostReactionRequest, Results<Ok<PostReactionsResponse>, NotFound>>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}/reactions");
    }

    public override async Task<Results<Ok<PostReactionsResponse>, NotFound>> ExecuteAsync(
        UpdatePostReactionRequest request, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.Where(post => post.Id == request.PostId)
            .Include(post => post.Reactions)
            .FirstOrDefaultAsync(post => post.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            Logger.LogWarning("Could not find post with ID {PostId} to add reaction to", request.PostId);
            return TypedResults.NotFound();
        }

        var postReaction = post.Reactions.FirstOrDefault(reaction => reaction.UserId == request.UserId);
        if (postReaction is not null)
        {
            Logger.LogInformation("Updating reaction to {Reaction} for user with ID {UserId} on post with ID {PostId}",
                request.Reaction, request.UserId, post.Id);
            postReaction.Reaction = request.Reaction;
        }
        else
        {
            Logger.LogInformation("Adding reaction {Reaction} for user with ID {UserId} to post with ID {PostId}",
                request.Reaction, request.UserId, post.Id);
            post.Reactions.Add(new PostReaction
            {
                UserId = request.UserId,
                Post = post,
                Reaction = request.Reaction
            });
        }

        dbContext.Add(PostUserActivity.UpdateReaction(request.UserId, request.PostId, request.Reaction));

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new PostReactionsResponse
        {
            LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
            FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
            CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
            WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
            SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
        };

        return TypedResults.Ok(response);
    }
}
