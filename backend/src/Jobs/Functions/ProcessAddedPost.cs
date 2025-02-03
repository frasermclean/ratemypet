using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core.Messages;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Jobs.Functions;

public class ProcessAddedPost(ApplicationDbContext dbContext)
{
    [Function(nameof(ProcessAddedPost))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.PostAdded)]
        PostAddedMessage message,
        CancellationToken cancellationToken)
    {
        // look up post by id
        var post = await dbContext.Posts.FirstAsync(post => post.Id == message.PostId, cancellationToken);

        // update post entity
        post.IsAnalyzed = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
