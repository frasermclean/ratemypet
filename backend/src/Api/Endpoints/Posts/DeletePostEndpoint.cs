using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostEndpoint(
    ApplicationDbContext dbContext,
    IImageHostingService imageHostingService) : Endpoint<DeletePostRequest, Results<NoContent, NotFound>>
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

        var result = await imageHostingService.SetAccessControlAsync(post.Image!.PublicId, false, cancellationToken);
        if (result.IsFailed)
        {
            throw new InvalidOperationException("Failed to set access control for post image");
        }

        post.DeletedAtUtc = DateTime.UtcNow;
        post.Activities.Add(PostUserActivity.DeletePost(request.UserId, request.PostId));
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Marked post ID {PostId} as deleted", request.PostId);

        return TypedResults.NoContent();
    }
}
