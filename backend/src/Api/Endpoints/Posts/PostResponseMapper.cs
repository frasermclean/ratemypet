using FastEndpoints;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Caption = post.Caption,
        Reactions = new PostReactionsResponse
        {
            LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
            CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
            FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
            WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
            SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
        }
    };
}
