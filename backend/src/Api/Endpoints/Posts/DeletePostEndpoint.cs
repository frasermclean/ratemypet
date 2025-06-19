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

        if (request.ShouldHardDelete.GetValueOrDefault() && User.IsInRole(Role.Administrator))
        {
            dbContext.Remove(post);
            Logger.LogInformation("Hard deleting post ID {PostId}", request.PostId);
        }
        else
        {
            post.DeletedAtUtc = DateTime.UtcNow;
            post.Activities.Add(PostUserActivity.DeletePost(request.UserId, request.PostId));
            Logger.LogInformation("Marking post ID {PostId} as deleted", request.PostId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (post.Image is not null)
        {
            await messagePublisher.PublishAsync(new PostDeletedMessage(post.Image.PublicId, request.ShouldHardDelete),
                cancellationToken);
        }

        return TypedResults.NoContent();
    }
}
