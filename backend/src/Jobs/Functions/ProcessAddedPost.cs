using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Core.Messages;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(
    ILogger<ProcessAddedPost> logger,
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager originalImagesManager,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager postImagesManager)
{
    [Function(nameof(ProcessAddedPost))]
    public async Task Execute(
        [QueueTrigger(QueueNames.PostAdded)] PostAddedMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing image for post with Id: {PostId}", message.PostId);

        var post = await dbContext.Posts
            .IgnoreQueryFilters()
            .FirstAsync(post => post.Id == message.PostId, cancellationToken);

        // get original image read stream
        await using var originalReadStream =
            await originalImagesManager.OpenReadStreamAsync(message.ImageBlobName, cancellationToken);

        await ProcessOriginalImageAsync(post, originalReadStream, cancellationToken);

        // update post entity in database
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

        await using var previewWriteStream = await postImagesManager.OpenWriteStreamAsync(
            post.GetImageBlobName(ImageSize.Preview), "image/jpeg", cancellationToken);
        await using var fullWriteStream = await postImagesManager.OpenWriteStreamAsync(
            post.GetImageBlobName(ImageSize.Full), "image/jpeg", cancellationToken);

        await previewImage.SaveAsJpegAsync(previewWriteStream, cancellationToken);
        await fullImage.SaveAsJpegAsync(fullWriteStream, cancellationToken);
    }
}
