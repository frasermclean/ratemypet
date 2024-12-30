using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;
using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager originalImagesManager,
    IMessagePublisher messagePublisher)
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

        // upload image to blob storage
        var blobName = $"{request.UserId}/{request.Image.FileName}";
        await originalImagesManager.CreateBlobAsync(blobName, request.Image.OpenReadStream(), request.Image.ContentType,
            cancellationToken);

        // create new post
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Species = species
        };

        // save the post entity
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        // publish message to queue
        await messagePublisher.PublishAsync(new PostAddedMessage
        {
            PostId = post.Id,
            ImageBlobName = blobName
        }, cancellationToken);

        return TypedResults.Created($"/posts/{post.Id}");
    }
}
