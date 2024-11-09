using FastEndpoints;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsEndpoint : EndpointWithoutRequest<IEnumerable<Post>>
{
    public override void Configure()
    {
        Get("posts");
        AllowAnonymous();
    }

    public override Task<IEnumerable<Post>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(SamplePosts);
    }

    private static readonly IEnumerable<Post> SamplePosts = new List<Post>
    {
        new() { Id = Guid.NewGuid(), Title = "First Post", Caption = "This is the first post" },
        new() { Id = Guid.NewGuid(), Title = "Second Post", Caption = "This is the second post" },
        new() { Id = Guid.NewGuid(), Title = "Third Post", Caption = "This is the third post" }
    };
}