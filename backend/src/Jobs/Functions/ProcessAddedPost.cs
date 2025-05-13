using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Database;
using RateMyPet.Infrastructure;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(
    ILogger<ProcessAddedPost> logger,
    ApplicationDbContext dbContext,
    IImageHostingService hostingService,
    IImageAnalysisService analysisService,
    IModerationService moderationService)
{
    [Function(nameof(ProcessAddedPost))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.PostAdded)] PostAddedMessage message,
        CancellationToken cancellationToken)
    {
        // look up post by id
        var post = await dbContext.Posts.FirstOrDefaultAsync(post => post.Id == message.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogError("Post {PostId} was not found", message.PostId);
            return;
        }

        var imagePublicId = post.Image?.PublicId;
        if (imagePublicId is null)
        {
            logger.LogError("Post {PostId} has no image", post.Id);
            return;
        }

        var imageUri = hostingService.GetPublicUri(imagePublicId);

        var moderationResults = await Task.WhenAll(
            moderationService.AnalyzeTextAsync(post.Title, cancellationToken),
            moderationService.AnalyzeTextAsync(post.Description, cancellationToken),
            moderationService.AnalyzeImageAsync(imageUri, cancellationToken)
        );

        if (moderationResults.All(result => result.IsSafe))
        {
            logger.LogInformation("Post {PostId} passed all moderation checks", post.Id);

            var tags = await analysisService.GetTagsAsync(imageUri, cancellationToken);

            post.Status = PostStatus.Approved;
            post.Tags = [.. post.Tags.Concat(tags).Distinct().Order()];
        }
        else
        {
            logger.LogWarning("Post {PostId} image detected as not safe", post.Id);
            post.Status = PostStatus.Rejected;
        }

        // update post entity
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
