using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;
using PostsPermissions = RateMyPet.Api.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager blobContainerManager) : Endpoint<DeletePostRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}");
        Permissions(PostsPermissions.DeleteOwned);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(DeletePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(post => post.Id == request.PostId, cancellationToken);
        if (post is null)
        {
            return TypedResults.NotFound();
        }

        await DeleteImageBlobAsync(post.Image.BlobName, cancellationToken);
        await RemoveEntityAsync(post, cancellationToken);

        return TypedResults.NoContent();
    }

    private async Task DeleteImageBlobAsync(string blobName, CancellationToken cancellationToken)
    {
        await blobContainerManager.DeleteBlobAsync(blobName, cancellationToken);
        Logger.LogInformation("Deleted image blob {BlobName}", blobName);
    }

    private async Task RemoveEntityAsync(Post post, CancellationToken cancellationToken)
    {
        var deletedReactionCount = await dbContext.PostReactions.Where(reaction => reaction.Post.Id == post.Id)
            .ExecuteDeleteAsync(cancellationToken);
        Logger.LogInformation("Deleted {DeletedReactionCount} reactions for post {PostId}", deletedReactionCount,
            post.Id);

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Deleted post {PostId}", post.Id);
    }
}
