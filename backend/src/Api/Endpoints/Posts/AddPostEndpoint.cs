using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)] IBlobContainerManager blobContainerManager)
    : Endpoint<AddPostRequest, PostResponse, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<PostResponse> ExecuteAsync(AddPostRequest request, CancellationToken cancellationToken)
    {
        // upload image to blob storage
        var stream = request.Image.OpenReadStream();
        var blobName = $"{request.UserId}/{request.Image.FileName}";
        await blobContainerManager.UploadBlobAsync(blobName, stream, request.Image.ContentType, cancellationToken);

        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);

        var post = new Post
        {
            Title = request.Title,
            Caption = request.Caption,
            User = user
        };

        user.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map.FromEntity(post);
    }
}
