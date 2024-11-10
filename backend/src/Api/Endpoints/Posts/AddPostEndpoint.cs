using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(ApplicationDbContext dbContext) : Endpoint<AddPostRequest, PostResponse>
{
    public override void Configure()
    {
        Post("posts");
    }

    public override async Task<PostResponse> ExecuteAsync(AddPostRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);

        var post = new Post
        {
            Title = request.Title,
            Caption = request.Caption,
            User = user
        };

        user.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Caption = post.Caption,
            Reactions = new PostReactionsResponse()
        };
    }
}
