using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class ImageProcessingOptions
{
    public const string SectionName = "ImageProcessing";

    [Required] public required string ContentType { get; init; }
    [Required] public int ImageWidth { get; init; }
    [Required] public int ImageHeight { get; init; }
    [Required] public required string ImagesContainerName { get; init; }
    [Required] public required string CacheContainerName { get; init; }
}
