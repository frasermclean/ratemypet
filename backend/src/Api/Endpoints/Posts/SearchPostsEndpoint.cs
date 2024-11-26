using Azure.Storage.Blobs;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Api.Services;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsEndpoint(
    ApplicationDbContext dbContext,
    EmailHasher emailHasher,
    BlobServiceClient blobServiceClient)
    : EndpointWithoutRequest<IEnumerable<PostResponse>>
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
                ImageUrl = blobServiceClient.GetBlobUri(post.Image.BlobName, BlobContainerNames.OriginalImages),
                AuthorEmailHash = emailHasher.GetSha256Hash(post.User.Email),
                LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
            })
            .ToListAsync(cancellationToken);
    }
}
