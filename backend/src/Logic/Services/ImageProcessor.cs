using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Logic.Services;

public class ImageProcessor(
    ILogger<ImageProcessor> logger,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager postImagesManager)
{
    public async Task ProcessOriginalImageAsync(Stream stream, Guid postId, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(stream, cancellationToken);
        logger.LogInformation("Read image successfully, dimensions: {Width}x{Height}", image.Width, image.Height);

        await SaveImageSizeAsync(image, ImageSize.Preview, Post.GetImageBlobName(postId, ImageSize.Preview),
            cancellationToken);
        await SaveImageSizeAsync(image, ImageSize.Full, Post.GetImageBlobName(postId, ImageSize.Full),
            cancellationToken);
    }

    private async Task SaveImageSizeAsync(Image image, ImageSize size, string blobName,
        CancellationToken cancellationToken)
    {
        var (width, height) = size switch
        {
            ImageSize.Preview => (320, 320),
            ImageSize.Full => (1024, 1024),
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        image.Clone(context => context
            .Resize(width, 0)
            .Crop(width, height)
        );

        await using var writeStream =
            await postImagesManager.OpenWriteStreamAsync(blobName, "image/jpeg", cancellationToken);
        await image.SaveAsJpegAsync(writeStream, cancellationToken);
    }
}
