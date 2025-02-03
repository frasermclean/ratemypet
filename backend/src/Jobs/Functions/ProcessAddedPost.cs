using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(
    ApplicationDbContext dbContext,
    IImageHostingService imageHostingService,
    IImageAnalysisService imageAnalysisService)
{
    [Function(nameof(ProcessAddedPost))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.PostAdded)] PostAddedMessage message,
        CancellationToken cancellationToken)
    {
        // look up post by id
        var post = await dbContext.Posts.FirstAsync(post => post.Id == message.PostId, cancellationToken);

        var imageUri = await imageHostingService.GetPublicUrl(message.ImagePublicId, cancellationToken);
        var tags = await imageAnalysisService.AnalyzeTagsAsync(imageUri, cancellationToken);

        // update post entity
        post.IsAnalyzed = true;
        post.Tags = post.Tags.Concat(tags).Distinct().Order().ToList();

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
