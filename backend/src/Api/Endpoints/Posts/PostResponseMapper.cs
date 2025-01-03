using FastEndpoints;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Extensions;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Description = post.Description,
        ImagePath = post.GetImagePath(),
        AuthorUserName = post.User.UserName!,
        AuthorEmailHash = post.User.Email.ToSha256Hash(),
        SpeciesName = post.Species.Name,
        CreatedAtUtc = post.CreatedAtUtc
    };
}
