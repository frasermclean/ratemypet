using Microsoft.Azure.Functions.Worker;
using RateMyPet.ImageHosting;
using RateMyPet.Storage;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Jobs.Functions;

public class ProcessDeletedPost(IImageHostingService imageHostingService)
{
    [Function(nameof(ProcessDeletedPost))]
    public async Task ExecuteAsync([QueueTrigger(QueueNames.PostDeleted)] PostDeletedMessage message,
        CancellationToken cancellationToken)
    {
        // take no action if the image is a sample image
        if (message.ImagePublicId.StartsWith("samples/"))
        {
            return;
        }

        // permanently delete the image if the message indicates a hard delete
        if (message.ShouldHardDelete.GetValueOrDefault())
        {
            await imageHostingService.DeleteAsync([message.ImagePublicId], cancellationToken);
            return;
        }

        await imageHostingService.SetAccessControlAsync(message.ImagePublicId, false, cancellationToken);
    }
}
