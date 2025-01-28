using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

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
        Roles(Role.Contributor);
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
        await imagesManager.CreateBlobAsync(postId.ToString(), imageStream, request.Image.ContentType,
            cancellationToken);



        // create new post
        var post = new Post
        {
            Id = postId,
            Slug = await CreateUniqueSlugAsync(request.Title, cancellationToken),
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species,
            Image = new PostImage
            {
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

    private async Task<string> CreateUniqueSlugAsync(string title, CancellationToken cancellationToken)
    {
        var slug = Core.Post.CreateSlug(title);
        var existingSlug = await dbContext.Posts
            .Where(p => p.Slug == slug)
            .Select(p => p.Slug)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSlug is null)
            return slug;

        var suffix = $"{Guid.NewGuid().ToString("N")[..4]}";
        return $"{slug}-{suffix}";
    }
}
