using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Jobs.Functions;

public class ProcessPostImage(
    ILogger<ProcessPostImage> logger,
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager postImagesContainerManager)
{
    [Function(nameof(ProcessPostImage))]
    public async Task Execute(
        [BlobTrigger($"{BlobContainerNames.OriginalImages}/{{name}}", Connection = "Storage")]
        BlobClient blobClient,
        CancellationToken cancellationToken)
    {
        var postId = Guid.Parse(blobClient.Name);
        logger.LogInformation("Processing image for post with Id: {PostId}", postId);

        var post = await dbContext.Posts
            .IgnoreQueryFilters()
            .FirstAsync(post => post.Id == postId, cancellationToken);

        await using var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        await ProcessOriginalImageAsync(post, stream, cancellationToken);

        post.IsProcessed = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessOriginalImageAsync(Post post, Stream stream, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(stream, cancellationToken);
        logger.LogInformation("Read image successfully, dimensions: {Width}x{Height}", image.Width, image.Height);

        var previewImage = image.Clone(context => context
            .Resize(320, 0, KnownResamplers.Lanczos3)
            .Crop(320, 320)
        );

        var fullImage = image.Clone(context => context
            .Resize(1024, 0)
            .Crop(1024, 1024)
        );

        await using var previewWriteStream = await postImagesContainerManager.OpenWriteStreamAsync(
            post.GetImageBlobName(ImageSize.Preview), "image/jpeg", cancellationToken);
        await using var fullWriteStream = await postImagesContainerManager.OpenWriteStreamAsync(
            post.GetImageBlobName(ImageSize.Full), "image/jpeg", cancellationToken);

        await previewImage.SaveAsJpegAsync(previewWriteStream, cancellationToken);
        await fullImage.SaveAsJpegAsync(fullWriteStream, cancellationToken);
    }
}
