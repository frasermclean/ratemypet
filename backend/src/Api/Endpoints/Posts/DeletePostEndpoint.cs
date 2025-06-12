using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Database;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostEndpoint(
    ApplicationDbContext dbContext,
    IMessagePublisher messagePublisher) : Endpoint<DeletePostRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}");
        Roles(Role.Contributor, Role.Administrator);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(DeletePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FindAsync([request.PostId], cancellationToken);

        Debug.Assert(post is not null); // pre-processor ensures this

        post.DeletedAtUtc = DateTime.UtcNow;
        post.Activities.Add(PostUserActivity.DeletePost(request.UserId, request.PostId));
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Marked post ID {PostId} as deleted", request.PostId);

        await messagePublisher.PublishAsync(new PostDeletedMessage(post.Id), cancellationToken);

        return TypedResults.NoContent();
    }
}
