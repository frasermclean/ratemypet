using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Database;
using RateMyPet.Storage;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager blobContainerManager,
    IMessagePublisher messagePublisher,
    IHostEnvironment hostEnvironment)
    : Endpoint<AddPostRequest, Created<PostResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        Roles(Role.Contributor);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Created<PostResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        // create new post
        var post = MapToPost(request);

        dbContext.Posts.Add(post);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (ReferenceConstraintException exception) when (exception.ConstraintProperties.Contains("SpeciesId"))
        {
            ThrowError(r => r.SpeciesId, "Invalid species ID");
        }

        // upload image to blob storage
        await blobContainerManager.CreateBlobAsync(post.Slug, request.Image.OpenReadStream(),
            request.Image.ContentType, cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        // publish message
        var message = new PostAddedMessage(post.Id, request.Image.FileName, hostEnvironment.EnvironmentName);
        await messagePublisher.PublishAsync(message, cancellationToken);

        var response = Map.FromEntity(post);
        return TypedResults.Created($"/posts/{response.Id}", response);
    }

    private static Post MapToPost(AddPostRequest request) => new(request.Title, request.UserId, request.SpeciesId)
    {
        Description = request.Description,
        Tags = request.Tags.Distinct().ToList(),
    };
}
