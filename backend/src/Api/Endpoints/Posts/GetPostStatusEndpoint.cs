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
        Get("posts/{postId}/status");
    }

    public override async Task<Results<Ok<PostStatus>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");

        try
        {
            var status = await dbContext.Posts.Where(post => post.Id == postId)
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
