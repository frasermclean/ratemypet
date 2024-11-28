using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Mapping;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostEndpoint(ApplicationDbContext dbContext, PostResponseMapper mapper)
    : Endpoint<UpdatePostRequest, Results<Ok<PostResponse>, NotFound, ErrorResponse>>
{
    public override void Configure()
    {
        Put("posts/{postId:guid}");
    }

    public override async Task<Results<Ok<PostResponse>, NotFound, ErrorResponse>> ExecuteAsync(
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

        var response = mapper.MapToResponse(post);
        return TypedResults.Ok(response);
    }
}
