using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdatePostRequest, Results<NoContent, NotFound, ErrorResponse, ForbidHttpResult>>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}");
        Roles(Role.Contributor);
        PreProcessor<ModifyPostPreProcessor>();
    }

    public override async Task<Results<NoContent, NotFound, ErrorResponse, ForbidHttpResult>> ExecuteAsync(
        UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.Where(p => p.Id == request.PostId)
            .Include(p => p.Reactions)
            .Include(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            return TypedResults.NotFound();
        }

        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ErrorResponse(ValidationFailures);
        }

        post.Title = request.Title;
        post.Description = request.Description;
        post.Species = species;
        post.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
