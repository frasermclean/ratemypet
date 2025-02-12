using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostStatusEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<Ok<PostStatus>, NotFound>>
{
    public override void Configure()
    {
        Get("posts/{postId:guid}/status", "posts/{postSlug}/status");
    }

    public override async Task<Results<Ok<PostStatus>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid?>("postId", false);
        var postSlug = Route<string?>("postSlug", false);

        try
        {
            var status = await dbContext.Posts.AsNoTracking()
                .Where(post => postId != null && post.Id == postId ||
                               postSlug != null && post.Slug == postSlug)
                .Select(post => post.Status)
                .FirstAsync(cancellationToken);

            return TypedResults.Ok(status);
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
    }
}
