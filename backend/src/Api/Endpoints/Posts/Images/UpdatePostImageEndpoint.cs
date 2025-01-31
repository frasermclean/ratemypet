using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;
using RateMyPet.Infrastructure.Services.ImageHosting;

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

        var result = await imageHostingService.GetAsync(request.ImageId, cancellationToken);
        if (result.IsFailed)
        {
            ThrowError(r => r.ImageId, "Invalid image ID");
        }

        post.Image = result.Value;
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Updated post with ID: {PostId} to use image with ID: {ImageId}", post.Id,
            request.ImageId);

        return TypedResults.NoContent();
    }
}
