using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager blobContainerManager) : EndpointWithoutRequest<Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");

        var post = await dbContext.Posts.FirstOrDefaultAsync(post => post.Id == postId, cancellationToken);
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
