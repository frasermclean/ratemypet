using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Logic.Services;
using RateMyPet.Persistence.Services;

using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    IPostImageProcessor imageProcessor)
    : Endpoint<AddPostRequest, Results<Created, ErrorResponse>>
{
    public override void Configure()
    {
        Post("posts");
        Permissions(PostsPermissions.Add);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Results<Created, ErrorResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ErrorResponse(ValidationFailures);
        }

        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            User = user,
            Species = species
        };

        await imageProcessor.ProcessOriginalImageAsync(request.Image.OpenReadStream(), post, cancellationToken);

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        return TypedResults.Created($"/posts/{post.Id}");
    }
}
