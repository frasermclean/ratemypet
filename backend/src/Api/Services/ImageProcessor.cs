using SixLabors.ImageSharp;

namespace RateMyPet.Api.Services;

public class ImageProcessor
{
    private const int MinimumSize = 1024;
    public const string ContentType = "image/webp";

    public async Task<ProcessImageResult> ProcessImageAsync(Stream readStream, Stream writeStream,
        CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(readStream, cancellationToken);
        var smallestDimension = Math.Min(image.Width, image.Height);

        if (smallestDimension < MinimumSize)
        {
            throw new InvalidOperationException("Image is too small");
        }

        await image.SaveAsWebpAsync(writeStream, cancellationToken);

        return new ProcessImageResult(image.Width, image.Height);
    }
}
