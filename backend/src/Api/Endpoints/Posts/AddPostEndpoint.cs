using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Services;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    ImageProcessor imageProcessor,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager blobContainerManager)
    : Endpoint<AddPostRequest, Created<PostResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Created<PostResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var imageResult = await ProcessAndUploadImageAsync(request, cancellationToken);
        var post = await CreatePostEntityAsync(request, imageResult, cancellationToken);

        return TypedResults.Created($"/posts/{post.Id}", Map.FromEntity(post));
    }

    private async Task<ProcessImageResult> ProcessAndUploadImageAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var blobName = imageProcessor.GenerateBlobName();

        await using var readStream = request.Image.OpenReadStream();
        await using var writeStream = await blobContainerManager.OpenWriteStreamAsync(blobName,
            imageProcessor.ContentType, cancellationToken);

        var result = await imageProcessor.ProcessImageAsync(readStream, writeStream, blobName, cancellationToken);

        Logger.LogInformation("Image processed and uploaded to blob storage, blobName: {BlobName}", blobName);

        return result;
    }

    private async Task<Post> CreatePostEntityAsync(AddPostRequest request, ProcessImageResult imageResult,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);
        var post = new Post
        {
            Title = request.Title,
            Caption = request.Caption,
            User = user,
            Image = new PostImage
            {
                Width = imageResult.Width,
                Height = imageResult.Height,
                BlobName = imageResult.BlobName,
                ContentType = imageResult.ContentType
            }
        };

        user.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        return post;
    }
}
