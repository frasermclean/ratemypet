using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;
using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

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

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Deleted post {PostId}", post.Id);

        await blobContainerManager.DeleteBlobAsync(request.PostId.ToString(), cancellationToken);

        return TypedResults.NoContent();
    }
}
