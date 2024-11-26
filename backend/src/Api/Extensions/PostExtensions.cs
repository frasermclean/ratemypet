using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Extensions;

public static class PostExtensions
{
    public static PostResponse ToResponse(this Post post, Uri imageUrl, string authorEmailHash, Guid userId) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Caption = post.Caption,
        ImageUrl = imageUrl,
        AuthorEmailHash = authorEmailHash,
        Reactions = new PostReactionsResponse
        {
            LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
            CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
            FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
            WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
            SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
        },
        UserReaction = post.Reactions.FirstOrDefault(reaction => reaction.User.Id == userId)!.Reaction
    };
}
