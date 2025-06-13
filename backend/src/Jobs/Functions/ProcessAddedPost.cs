using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Database;
using RateMyPet.Storage;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(
    ILogger<ProcessAddedPost> logger,
    ApplicationDbContext dbContext,
    IImageHostingService imageHostingService,
    IImageAnalysisService analysisService,
    IModerationService moderationService,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager blobContainerManager)
{
    [Function(nameof(ProcessAddedPost))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.PostAdded)] PostAddedMessage message,
        CancellationToken cancellationToken)
    {
        // look up post by id
        var post = await dbContext.Posts.FirstAsync(post => post.Id == message.PostId, cancellationToken);

        // get stream from blob storage
        await using var stream = await blobContainerManager.OpenReadStreamAsync(post.Slug, cancellationToken);
        var imageData = await BinaryData.FromStreamAsync(stream, cancellationToken);

        var isSafe = await ModerateContentAsync(post, imageData, cancellationToken);
        if (isSafe)
        {
            logger.LogInformation("Post {PostId} passed all moderation checks", post.Id);

            var tags = await analysisService.GetTagsAsync(imageData, cancellationToken);

            stream.Position = 0; // reset stream position after reading

            post.Status = PostStatus.Approved;
            post.Tags = [.. post.Tags.Concat(tags).Distinct().Order()];
            post.Image = await imageHostingService.UploadAsync(message.ImageFileName, stream, post, cancellationToken);
        }
        else
        {
            logger.LogWarning("Post {PostId} image detected as not safe", post.Id);
            post.Status = PostStatus.Rejected;
        }

        // update post entity
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> ModerateContentAsync(Post post, BinaryData imageData, CancellationToken cancellationToken)
    {
        var results = await Task.WhenAll(
            moderationService.AnalyzeTextAsync(post.Title, cancellationToken),
            moderationService.AnalyzeTextAsync(post.Description, cancellationToken),
            moderationService.AnalyzeImageAsync(imageData, cancellationToken)
        );

        return results.All(result => result.IsSafe);
    }
}
