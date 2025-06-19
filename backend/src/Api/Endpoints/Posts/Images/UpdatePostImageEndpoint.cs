using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;
using RateMyPet.ImageHosting;

namespace RateMyPet.Api.Endpoints.Posts.Images;

public class UpdatePostImageEndpoint(ApplicationDbContext dbContext, IImageHostingService imageHostingService)
    : Endpoint<UpdatePostImageRequest, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}/imageId");
        Roles(Role.Administrator);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(
        UpdatePostImageRequest request, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(post => post.Id == request.PostId, cancellationToken);
        if (post is null)
        {
            return TypedResults.NotFound();
        }

        post.Image = await imageHostingService.GetAsync(request.ImageId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Updated post with ID: {PostId} to use image with ID: {ImageId}", post.Id,
            request.ImageId);

        return TypedResults.NoContent();
    }
}
