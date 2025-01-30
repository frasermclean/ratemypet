using FastEndpoints;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Slug = post.Slug,
        Title = post.Title,
        Description = post.Description,
        ImageId = post.Image?.PublicId,
        AuthorUserName = post.User.UserName!,
        AuthorEmailHash = post.User.Email.ToSha256Hash(),
        SpeciesId = post.Species.Id,
        CreatedAt = post.CreatedAtUtc,
        UpdatedAt = post.UpdatedAtUtc,
    };
}
