using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;
using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    IPostImageProcessor imageProcessor,
    [FromKeyedServices(BlobContainerNames.Images)]
    IBlobContainerManager imagesManager)
    : Endpoint<AddPostRequest, Results<Created<PostResponse>, ErrorResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        Permissions(PostsPermissions.Add);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Results<Created<PostResponse>, ErrorResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ErrorResponse(ValidationFailures);
        }

        // validate image
        await using var imageStream = request.Image.OpenReadStream();
        var imageValidationResult = await imageProcessor.ValidateImageAsync(imageStream, cancellationToken);
        if (imageValidationResult.IsFailed)
        {
            AddError(r => r.Image, imageValidationResult.Errors.First().Message);
            return new ErrorResponse(ValidationFailures);
        }

        var postId = Guid.NewGuid();
        var (width, height) = imageValidationResult.Value;

        // upload image to blob storage
        imageStream.Position = 0;
        var blobName = $"{postId}/original";
        await imagesManager.CreateBlobAsync(blobName, imageStream, request.Image.ContentType, cancellationToken);

        // create new post
        var post = new Post
        {
            Id = postId,
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species,
            Image = new PostImage
            {
                BlobName = blobName,
                FileName = request.Image.FileName,
                MimeType = request.Image.ContentType,
                Width = width,
                Height = height,
                Size = request.Image.Length
            }
        };

        // save the post entity
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        var response = Map.FromEntity(post);
        return TypedResults.Created($"/posts/{response.Id}", response);
    }
}
