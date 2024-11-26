using Microsoft.Extensions.Options;
using RateMyPet.Api.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Api.Services;

public class ImageProcessor(IOptions<ImageProcessorOptions> options)
{
    private readonly ImageProcessorOptions options = options.Value;

    public string ContentType => options.ContentType;

    public string GenerateBlobName()
    {
        var extension = ContentType switch
        {
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };

        return $"{Guid.NewGuid():N}.{extension}";
    }

    public async Task<ProcessImageResult> ProcessImageAsync(Stream readStream, Stream writeStream,
        string blobName, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(readStream, cancellationToken);
        image.Mutate(context => context.Resize(options.FullWidth, 0));

        var saveTask = ContentType switch
        {
            "image/png" => image.SaveAsPngAsync(writeStream, cancellationToken),
            "image/webp" => image.SaveAsWebpAsync(writeStream, cancellationToken),
            _ => image.SaveAsJpegAsync(writeStream, cancellationToken)
        };

        await saveTask;

        return new ProcessImageResult(image.Width, image.Height, ContentType, blobName);
    }
}
