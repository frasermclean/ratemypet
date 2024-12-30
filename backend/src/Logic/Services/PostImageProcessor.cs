using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;
using RateMyPet.Logic.Errors;
using RateMyPet.Logic.Options;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Logic.Services;

public interface IPostImageProcessor
{
    Task<Result> ProcessOriginalImageAsync(Stream stream, Post post, CancellationToken cancellationToken = default);
}

public class PostImageProcessor(
    IOptions<ImageProcessingOptions> options,
    ILogger<PostImageProcessor> logger,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager containerManager) : IPostImageProcessor
{
    private readonly string contentType = options.Value.ContentType;
    private readonly int previewWidth = options.Value.PreviewWidth;
    private readonly int previewHeight = options.Value.PreviewHeight;
    private readonly int fullWidth = options.Value.FullWidth;
    private readonly int fullHeight = options.Value.FullHeight;

    private readonly IImageEncoder imageEncoder = options.Value.ContentType switch
    {
        "image/png" => Configuration.Default.ImageFormatsManager.GetEncoder(PngFormat.Instance),
        "image/webp" => Configuration.Default.ImageFormatsManager.GetEncoder(WebpFormat.Instance),
        _ => Configuration.Default.ImageFormatsManager.GetEncoder(JpegFormat.Instance)
    };

    public async Task<Result> ProcessOriginalImageAsync(Stream stream, Post post,
        CancellationToken cancellationToken = default)
    {
        var loadResult = await Result.Try(
            () => Image.LoadAsync(stream, cancellationToken),
            ImageProcessingError.FromException);

        if (loadResult.IsFailed)
        {
            logger.LogError("Failed to load image from stream");
            return loadResult.ToResult();
        }

        using var image = loadResult.Value;

        // validate image dimensions
        if (image.Width < fullWidth || image.Height < fullHeight)
        {
            logger.LogError("Image dimensions are too small, dimensions: {Width}x{Height}", image.Width,
                image.Height);
            return new ImageTooSmallError(image.Width, image.Height);
        }

        logger.LogInformation("Read image successfully, dimensions: {Width}x{Height}", image.Width, image.Height);

        await ResizeAndSaveImageAsync(image, previewWidth, previewHeight, post.GetImageBlobName(ImageSize.Preview),
            cancellationToken);
        await ResizeAndSaveImageAsync(image, fullWidth, fullHeight, post.GetImageBlobName(ImageSize.Full),
            cancellationToken);

        return Result.Ok();
    }

    private async Task ResizeAndSaveImageAsync(Image image, int width, int height, string blobName,
        CancellationToken cancellationToken)
    {
        // crop and resize image
        using var resizedImage = image.Clone(context => context
            .Crop(width, height)
            .Resize(width, 0)
        );

        await using var stream = await containerManager.OpenWriteStreamAsync(blobName, contentType, cancellationToken);

        await resizedImage.SaveAsync(stream, imageEncoder, cancellationToken);
    }
}
