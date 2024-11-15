﻿using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Services;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostEndpoint(ApplicationDbContext dbContext, EmailHasher emailHasher)
    : EndpointWithoutRequest<Results<Ok<PostResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("posts/{postId:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<PostResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");

        var response = await dbContext.Posts
            .AsNoTracking()
            .Where(post => post.Id == postId)
            .Select(post => new PostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Caption = post.Caption,
                ImageUrl = PostResponse.SampleImageUrl,
                AuthorEmailHash = emailHasher.GetSha256Hash(post.User.Email),
                LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.NotFound();
    }
}
