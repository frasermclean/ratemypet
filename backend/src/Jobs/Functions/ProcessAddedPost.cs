using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(
    ILogger<ProcessAddedPost> logger,
    ApplicationDbContext dbContext,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager originalImagesManager,
    IPostImageProcessor imageProcessor)
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

        // process the image
        var imageResult = await imageProcessor.ProcessOriginalImageAsync(originalReadStream, post, cancellationToken);
        if (imageResult.IsFailed)
        {
            throw new InvalidOperationException("Failed to process image");
        }

        // update post entity in database
        post.IsProcessed = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
