using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RateMyPet.Api.Services;

public class ImageProcessor
{
    private const int MinimumSize = 1024;

    public async Task ProcessImageAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(stream, cancellationToken);
        var smallestDimension = Math.Min(image.Width, image.Height);

        if (smallestDimension < MinimumSize)
        {
            return;
        }
    }
}
