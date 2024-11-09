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
            Reactions = new PostReactionResponse
            {
                LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
            }
        };
    }
}