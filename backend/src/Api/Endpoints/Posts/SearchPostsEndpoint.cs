using Azure.Storage.Blobs;
using FastEndpoints;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http.HttpResults;
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
    : Endpoint<GridifyQuery, Results<Ok<Paging<SearchPostsMatch>>, BadRequest>>
{
    public override void Configure()
    {
        Get("posts");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Paging<SearchPostsMatch>>, BadRequest>> ExecuteAsync(GridifyQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.IsValid<SearchPostsMatch>())
        {
            return TypedResults.BadRequest();
        }

        var userId = User.GetUserId();
        var paging = await dbContext.Posts
            .AsNoTracking()
            .Select(post => new SearchPostsMatch
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ImageUrl = blobServiceClient.GetBlobUri(post.Image.BlobName, BlobContainerNames.OriginalImages),
                AuthorUserName = post.User.UserName!,
                AuthorEmailHash = emailHasher.GetSha256Hash(post.User.Email),
                SpeciesName = post.Species.Name,
                CreatedAtUtc = post.CreatedAtUtc,
                UpdatedAtUtc = post.UpdatedAtUtc,
                Reactions = new PostReactionsResponse
                {
                    LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                    CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                    FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                    WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                    SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
                },
                UserReaction = post.Reactions.FirstOrDefault(reaction => reaction.User.Id == userId)!.Reaction
            })
            .GridifyAsync(query, cancellationToken);

        return TypedResults.Ok(paging);
    }
}
