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
    IBlobContainerManager imagesManager) : Endpoint<DeletePostRequest, NoContent>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}");
        Roles(Role.Contributor, Role.Administrator);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<NoContent> ExecuteAsync(DeletePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FirstAsync(post => post.Id == request.PostId, cancellationToken);

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Deleted post {PostId}", post.Id);

        await imagesManager.DeleteBlobAsync(post.Id.ToString(), cancellationToken);

        return TypedResults.NoContent();
    }
}
