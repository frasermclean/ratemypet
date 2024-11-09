using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsEndpoint(ApplicationDbContext dbContext) : EndpointWithoutRequest<IEnumerable<PostResponse>>
{
    public override void Configure()
    {
        Get("posts");
        AllowAnonymous();
    }

    public override async Task<IEnumerable<PostResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Posts
            .AsNoTracking()
            .Select(post => new PostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Caption = post.Caption,
                Reactions = post.Reactions
            })
            .ToListAsync(cancellationToken);
    }
}