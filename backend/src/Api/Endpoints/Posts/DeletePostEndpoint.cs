using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.Images)]
    IBlobContainerManager imagesManager) : Endpoint<DeletePostRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}");
        Roles(Role.Contributor);
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

        await imagesManager.DeleteBlobAsync(post.Id.ToString(), cancellationToken);

        return TypedResults.NoContent();
    }
}
