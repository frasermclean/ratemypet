using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPopularTagsEndpoint(ApplicationDbContext dbContext) : EndpointWithoutRequest<IEnumerable<string>>
{
    public override void Configure()
    {
        Get("posts/popular-tags");
        AllowAnonymous();
    }

    public override async Task<IEnumerable<string>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var count = Query<int?>("count", false) ?? 10;

        var tags = await dbContext.Posts
            .SelectMany(post => post.Tags)
            .GroupBy(tag => tag)
            .OrderByDescending(group => group.Count())
            .Take(count)
            .Select(grouping => grouping.Key)
            .ToListAsync(cancellationToken);

        return tags;
    }
}
