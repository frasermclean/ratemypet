using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostRequest, Results<Ok<PostResponse>, ErrorResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}");
        Roles(Role.Contributor, Role.Administrator);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<Results<Ok<PostResponse>, ErrorResponse>> ExecuteAsync(
        UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.Where(p => p.Id == request.PostId)
            .Include(p => p.Reactions)
            .Include(p => p.User)
            .FirstAsync(cancellationToken);

        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ErrorResponse(ValidationFailures);
        }

        post.Description = request.Description;
        post.Species = species;
        post.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = Map.FromEntity(post);
        return TypedResults.Ok(response);
    }
}
