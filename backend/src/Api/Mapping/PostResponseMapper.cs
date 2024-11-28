using Azure.Storage.Blobs;
using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Api.Extensions;
using RateMyPet.Api.Services;
using RateMyPet.Persistence.Models;
using Riok.Mapperly.Abstractions;

namespace RateMyPet.Api.Mapping;

[Mapper]
public partial class PostResponseMapper(
    BlobServiceClient blobServiceClient,
    EmailHasher emailHasher,
    IHttpContextAccessor httpContextAccessor)
{
    [MapProperty(nameof(Post.Image.BlobName), nameof(PostResponse.ImageUrl))]
    [MapProperty(nameof(Post.User.Email), nameof(PostResponse.AuthorEmailHash), Use = nameof(MapAuthorEmailHash))]
    [MapProperty(nameof(Post.Reactions), nameof(PostResponse.UserReaction))]
    [MapperIgnoreSource(nameof(Post.RowVersion))]
    public partial PostResponse MapToResponse(Post post);

    private Uri MapImageUrl(string blobName) => blobServiceClient.GetBlobUri(blobName);

    [UserMapping(Default = false)]
    private string MapAuthorEmailHash(string emailAddress) => emailHasher.GetSha256Hash(emailAddress);

    private Reaction? MapUserReaction(List<PostReaction> reactions)
    {
        var userId = httpContextAccessor.HttpContext?.User.GetUserId();
        return userId is not null
            ? reactions.FirstOrDefault(reaction => reaction.User.Id == userId)?.Reaction
            : null;
    }

    private static PostReactionsResponse MapReactions(List<PostReaction> reactions) => new()
    {
        LikeCount = reactions.Count(reaction => reaction.Reaction == Reaction.Like),
        CrazyCount = reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
        FunnyCount = reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
        WowCount = reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
        SadCount = reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
    };
}
