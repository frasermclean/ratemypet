﻿using Azure.Storage.Blobs;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostEndpoint(
    ApplicationDbContext dbContext,
    BlobServiceClient blobServiceClient)
    : EndpointWithoutRequest<Results<Ok<GetPostResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("posts/{postId:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetPostResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");
        var userId = User.GetUserId();

        var response = await dbContext.Posts.Where(post => post.Id == postId)
            .Select(post => new GetPostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ImageUrl = blobServiceClient.GetBlobUri(post.GetImageBlobName(ImageSize.Full),
                    BlobContainerNames.PostImages),
                AuthorUserName = post.User.UserName!,
                AuthorEmailHash = post.User.Email.ToSha256Hash(),
                SpeciesName = post.Species.Name,
                CreatedAtUtc = post.CreatedAtUtc,
                UpdatedAtUtc = post.UpdatedAtUtc,
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
                    }).ToCommentTree(),
                CommentCount = post.Comments.Count
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.NotFound();
    }
}
