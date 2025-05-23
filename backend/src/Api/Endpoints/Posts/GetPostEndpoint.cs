﻿using Delta;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostEndpoint(ApplicationDbContext dbContext)
    : Endpoint<GetPostRequest, Results<Ok<PostResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("posts/{postId:guid}", "posts/{postSlug}");
        Options(builder => builder.UseDelta<ApplicationDbContext>());
        AllowAnonymous();
    }

    public override async Task<Results<Ok<PostResponse>, NotFound>> ExecuteAsync(GetPostRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var response = await dbContext.Posts.AsNoTracking()
            .Where(post => request.PostId != null && post.Id == request.PostId ||
                           request.PostSlug != null && post.Slug == request.PostSlug)
            .Select(post => new PostResponse
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Description = post.Description,
                ImageId = post.Image != null ? post.Image.PublicId : null,
                AuthorUserName = post.User.UserName!,
                AuthorEmailHash = post.User.Email.ToSha256Hash(),
                SpeciesId = post.Species.Id,
                Tags = post.Tags,
                Status = post.Status,
                CreatedAt = post.CreatedAtUtc,
                UpdatedAt = post.UpdatedAtUtc,
                UserReaction = post.Reactions.FirstOrDefault(reaction => reaction.User.Id == userId)!.Reaction,
                Reactions = new PostReactionsResponse
                {
                    LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                    CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                    FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                    WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                    SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
                },
                Comments = post.Comments.OrderBy(comment => comment.Parent)
                    .ThenBy(comment => comment.CreatedAtUtc)
                    .Select(comment => new PostCommentResponse
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        AuthorUserName = comment.User.UserName!,
                        CreatedAtUtc = comment.CreatedAtUtc,
                        UpdatedAtUtc = comment.UpdatedAtUtc,
                        ParentId = comment.Parent == null ? null : comment.Parent.Id,
                    }).ToCommentTree()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.NotFound();
    }
}
