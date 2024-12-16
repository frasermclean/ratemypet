using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SpeciesModel = RateMyPet.Core.Species;
using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager blobContainerManager)
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

        var post = await CreatePostEntityAsync(request, species, cancellationToken);

        await blobContainerManager.CreateBlobAsync(
            blobName: post.Id.ToString(),
            content: request.Image.OpenReadStream(),
            contentType: request.Image.ContentType,
            cancellationToken: cancellationToken);

        return TypedResults.Created($"/posts/{post.Id}");
    }

    private async Task<Post> CreatePostEntityAsync(AddPostRequest request, SpeciesModel species,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            User = user,
            Species = species
        };

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        return post;
    }
}
