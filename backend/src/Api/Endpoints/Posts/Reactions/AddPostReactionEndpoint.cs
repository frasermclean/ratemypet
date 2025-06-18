using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class AddPostReactionEndpoint(ApplicationDbContext dbContext) : Endpoint<AddPostReactionRequest, NoContent>
{
    public override void Configure()
    {
        Post("/posts/{postId:guid}/reactions");
        Roles(Role.Contributor);
    }

    public override async Task<NoContent> ExecuteAsync(AddPostReactionRequest request,
        CancellationToken cancellationToken)
    {
        var postReaction = MapToPostReaction(request);
        dbContext.Add(postReaction);
        dbContext.Add(PostUserActivity.AddReaction(request.UserId, request.PostId, request.Reaction));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException exception) when (exception.ConstraintName == "PK_PostReactions")
        {
            Logger.LogError(exception, "Reaction already exists for user {UserId} on post {PostId}",
                request.UserId, request.PostId);

            ThrowError(r => r.Reaction, "You have already reacted to this post.");
        }

        Logger.LogInformation("Added reaction {Reaction} for user with ID {UserId} on post with ID {PostId}",
            request.Reaction, request.UserId, request.PostId);

        return TypedResults.NoContent();
    }

    private static PostReaction MapToPostReaction(AddPostReactionRequest request) => new()
    {
        PostId = request.PostId,
        UserId = request.UserId,
        Reaction = request.Reaction
    };
}
