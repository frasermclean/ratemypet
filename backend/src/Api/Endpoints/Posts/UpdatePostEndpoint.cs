using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostRequest, Ok<PostResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}");
        Roles(Role.Contributor, Role.Administrator);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<Ok<PostResponse>> ExecuteAsync(
        UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.Where(p => p.Id == request.PostId)
            .Include(p => p.Reactions)
            .Include(p => p.User)
            .FirstAsync(cancellationToken);

        post.Description = request.Description;
        post.SpeciesId = request.SpeciesId;
        post.UpdatedAtUtc = DateTime.UtcNow;
        post.Tags = request.Tags.Distinct().ToList();
        post.Activities.Add(PostUserActivity.UpdatePost(request.UserId, post.Id));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (ReferenceConstraintException exception) when (exception.ConstraintProperties.Contains("SpeciesId"))
        {
            ThrowError(r => r.SpeciesId, "Invalid species ID");
        }

        var response = Map.FromEntity(post);
        return TypedResults.Ok(response);
    }
}
