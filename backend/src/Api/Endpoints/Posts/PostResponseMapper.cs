﻿using FastEndpoints;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponseMapper(IHttpContextAccessor httpContextAccessor) : ResponseMapper<PostResponse, Post>
{
    public override PostResponse FromEntity(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Description = post.Description,
        ImageUrl = post.GetImageUrl(httpContextAccessor.HttpContext?.Request),
        AuthorUserName = post.User.UserName!,
        AuthorEmailHash = post.User.Email.ToSha256Hash(),
        SpeciesId = post.Species.Id,
        CreatedAt = post.CreatedAtUtc,
        UpdatedAt = post.UpdatedAtUtc,
    };
}
