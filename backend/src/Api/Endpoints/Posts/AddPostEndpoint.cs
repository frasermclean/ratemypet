using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Services;
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

        // create new post
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species
        };

        // process the image
        var imageStream = request.Image.OpenReadStream();
        var imageResult = await imageProcessor.ProcessOriginalImageAsync(imageStream, post, cancellationToken);
        if (imageResult.IsFailed)
        {
            ValidationFailures.AddRange(imageResult.Errors.ToValidationFailures(nameof(request.Image)));
            return new ErrorResponse(ValidationFailures);
        }

        // save the post entity
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        return TypedResults.Created($"/posts/{post.Id}");
    }
}
