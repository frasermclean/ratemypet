using Azure.Storage.Blobs;
using FastEndpoints;
using RateMyPet.Api.Extensions;
using RateMyPet.Api.Services;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper(EmailHasher emailHasher, BlobServiceClient blobServiceClient) : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Caption = post.Caption,
        ImageUrl = blobServiceClient.GetBlobUri(post.Image.BlobName),
        AuthorEmailHash = emailHasher.GetSha256Hash(post.User.Email),
        LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
        CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
        FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
        WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
        SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
    };
}
