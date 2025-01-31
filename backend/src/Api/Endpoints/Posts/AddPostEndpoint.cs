using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Infrastructure.Services;
using RateMyPet.Infrastructure.Services.ImageHosting;
using Role = RateMyPet.Core.Role;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(ApplicationDbContext dbContext, IImageHostingService imageHostingService)
    : Endpoint<AddPostRequest, Results<Created<PostResponse>, ProblemDetails>, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        Roles(Role.Contributor);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Results<Created<PostResponse>, ProblemDetails>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ProblemDetails(ValidationFailures);
        }

        // create new post
        var post = new Post
        {
            Slug = Core.Post.CreateSlug(request.Title),
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species
        };

        // upload image to cloudinary
        var imageUploadResult = await imageHostingService.UploadAsync(request.Image.FileName,
            request.Image.OpenReadStream(), post, cancellationToken);

        if (imageUploadResult.IsFailed)
        {
            return imageUploadResult.ToProblemDetails("image");
        }

        post.Image = imageUploadResult.Value;

        // save the post entity
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        var response = Map.FromEntity(post);
        return TypedResults.Created($"/posts/{response.Id}", response);
    }
}
