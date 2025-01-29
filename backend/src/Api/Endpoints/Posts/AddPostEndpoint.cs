using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;
using RateMyPet.Infrastructure.Services.ImageHosting;
using Role = RateMyPet.Core.Role;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(ApplicationDbContext dbContext, IImageHostingService imageHostingService)
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

        var slug = Core.Post.CreateSlug(request.Title);

        // upload image to cloudinary
        var imageUploadResult = await imageHostingService.UploadAsync(request.Image.FileName,
            request.Image.OpenReadStream(), request.Title, slug, cancellationToken);

        // create new post
        var post = new Post
        {
            Slug = slug,
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species,
            Image = new PostImage
            {
                AssetId = imageUploadResult.AssetId,
                PublicId = imageUploadResult.PublicId
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
