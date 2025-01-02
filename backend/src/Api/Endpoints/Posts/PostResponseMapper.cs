using Azure.Storage.Blobs;
using FastEndpoints;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Infrastructure;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper(BlobServiceClient blobServiceClient) : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Description = post.Description,
        ImageUrl = blobServiceClient.GetBlobUri(post.Image.FullBlobName, BlobContainerNames.PostImages),
        AuthorUserName = post.User.UserName!,
        AuthorEmailHash = post.User.Email.ToSha256Hash(),
        SpeciesName = post.Species.Name,
        Status = post.Status,
        CreatedAtUtc = post.CreatedAtUtc
    };
}
