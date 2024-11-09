using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Persistence;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsEndpoint(ApplicationDbContext dbContext) : EndpointWithoutRequest<IEnumerable<Post>>
{
    public override void Configure()
    {
        Get("posts");
        AllowAnonymous();
    }

    public override async Task<IEnumerable<Post>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var posts = await dbContext.Posts.ToListAsync(cancellationToken);
        return posts;
    }
}