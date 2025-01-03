using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Errors;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class PostImageProcessor(
    IOptions<ImageProcessingOptions> options,
    ILogger<PostImageProcessor> logger,
    [FromKeyedServices(BlobContainerNames.PostImages)]
    IBlobContainerManager containerManager) : IPostImageProcessor
{
    private readonly string contentType = options.Value.ContentType;
    private readonly int imageWidth = options.Value.ImageWidth;
    private readonly int imageHeight = options.Value.ImageHeight;

    private readonly IImageEncoder imageEncoder = options.Value.ContentType switch
    {
        "image/png" => SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.GetEncoder(PngFormat.Instance),
        "image/webp" => SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.GetEncoder(WebpFormat.Instance),
        _ => SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.GetEncoder(JpegFormat.Instance)
    };

    private readonly string fileExtension = options.Value.ContentType switch
    {
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => ".jpg"
    };

    public string GetBlobName(Guid postId) => $"{postId}/processed{fileExtension}";

    public async Task<Result<(int Width, int Height)>> ValidateImageAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        var identifyResult = await Result.Try(
            () => Image.IdentifyAsync(stream, cancellationToken),
            ImageProcessingError.FromException);

        if (identifyResult.IsFailed)
        {
            logger.LogError("Failed to identify image from stream");
            return identifyResult.ToResult();
        }

        var imageInfo = identifyResult.Value;
        return (imageInfo.Width, imageInfo.Height);
    }

    public async Task<Result<string>> ProcessOriginalImageAsync(Stream stream, Post post,
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
        if (image.Width < imageWidth || image.Height < imageHeight)
        {
            logger.LogError("Image dimensions are too small, dimensions: {Width}x{Height}", image.Width,
                image.Height);
            return new ImageTooSmallError(image.Width, image.Height);
        }

        logger.LogInformation("Read image successfully, dimensions: {Width}x{Height}", image.Width, image.Height);

        var blobName = GetBlobName(post.Id);
        await ResizeAndSaveImageAsync(image, imageWidth, imageHeight, blobName, cancellationToken);

        return blobName;
    }

    private async Task ResizeAndSaveImageAsync(Image source, int width, int height, string blobName,
        CancellationToken cancellationToken)
    {
        // create square, centered crop rectangle
        var cropRectangle = source.Width >= source.Height
            ? new Rectangle((source.Width - source.Height) / 2, 0, source.Height, source.Height)
            : new Rectangle(0, (source.Height - source.Width) / 2, source.Width, source.Width);

        // crop and resize source image
        using var resizedImage = source.Clone(context => context
            .Crop(cropRectangle)
            .Resize(width, height)
        );

        await using var stream = await containerManager.OpenWriteStreamAsync(blobName, contentType, cancellationToken);

        await resizedImage.SaveAsync(stream, imageEncoder, cancellationToken);
    }
}
