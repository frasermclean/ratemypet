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
    [FromKeyedServices(BlobContainerNames.Images)]
    IBlobContainerManager imagesManager,
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
        await using var readStream = await imagesManager.OpenReadStreamAsync(message.ImageBlobName, cancellationToken);

        // process the image
        var imageResult = await imageProcessor.ProcessOriginalImageAsync(readStream, post, cancellationToken);

        // update post entity in database
        if (imageResult.IsSuccess)
        {
            post.Image.IsProcessed = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            logger.LogError("Failed to process image for post with Id: {PostId}", message.PostId);
        }
    }
}
