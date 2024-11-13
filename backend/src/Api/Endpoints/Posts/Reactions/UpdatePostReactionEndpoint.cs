﻿using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class UpdatePostReactionEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostReactionRequest, Results<Ok<PostResponse>, NotFound>, PostResponseMapper>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}/reactions");
    }

    public override async Task<Results<Ok<PostResponse>, NotFound>> ExecuteAsync(UpdatePostReactionRequest request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.Where(post => post.Id == request.PostId)
            .Include(post => post.Reactions)
            .FirstOrDefaultAsync(post => post.Id == request.PostId, cancellationToken);
        if (post is null)
        {
            Logger.LogWarning("Could not find post with ID {PostId} to add reaction to", request.PostId);
            return TypedResults.NotFound();
        }

        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);

        var existingReaction = post.Reactions.FirstOrDefault(reaction => reaction.User.Id == user.Id);
        if (existingReaction is not null)
        {
            Logger.LogInformation("Updating reaction to {Reaction} for user with ID {UserId} on post with ID {PostId}",
                request.Reaction, user.Id, post.Id);
            existingReaction.Reaction = request.Reaction;
        }
        else
        {
            Logger.LogInformation("Adding reaction {Reaction} for user with ID {UserId} to post with ID {PostId}",
                request.Reaction, user.Id, post.Id);
            post.Reactions.Add(new PostReaction
            {
                User = user,
                Post = post,
                Reaction = request.Reaction
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(Map.FromEntity(post));
    }
}