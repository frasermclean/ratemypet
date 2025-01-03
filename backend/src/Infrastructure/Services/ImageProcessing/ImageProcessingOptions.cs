using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class ImageProcessingOptions
{
    public const string SectionName = "ImageProcessing";

    [Required] public required string ContentType { get; init; }
    [Required, Range(256, 1024)] public int PreviewWidth { get; init; }
    [Required, Range(256, 1024)] public int PreviewHeight { get; init; }
    [Required, Range(256, 2048)] public int FullWidth { get; init; }
    [Required, Range(256, 2048)] public int FullHeight { get; init; }

    [Required] public required string CacheContainerName { get; init; }
    [Required, Range(2, 64)] public int CacheHashLength { get; init; }
}
