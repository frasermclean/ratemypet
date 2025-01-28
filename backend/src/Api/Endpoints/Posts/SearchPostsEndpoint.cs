using FastEndpoints;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsEndpoint(ApplicationDbContext dbContext)
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
            .Select(post => new SearchPostsMatch
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Description = post.Description,
                ImageId = post.Image.PublicId,
                AuthorUserName = post.User.UserName!,
                AuthorEmailHash = post.User.Email.ToSha256Hash(),
                SpeciesName = post.Species.Name,
                CreatedAt = post.CreatedAtUtc,
                UpdatedAt = post.UpdatedAtUtc,
                Reactions = new PostReactionsResponse
                {
                    LikeCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Like),
                    CrazyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Crazy),
                    FunnyCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Funny),
                    WowCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Wow),
                    SadCount = post.Reactions.Count(reaction => reaction.Reaction == Reaction.Sad)
                },
                ReactionCount = post.Reactions.Count,
                UserReaction = post.Reactions.FirstOrDefault(reaction => reaction.User.Id == userId)!.Reaction,
                CommentCount = post.Comments.Count
            })
            .AsNoTracking()
            .GridifyAsync(query, cancellationToken);

        return TypedResults.Ok(paging);
    }
}
