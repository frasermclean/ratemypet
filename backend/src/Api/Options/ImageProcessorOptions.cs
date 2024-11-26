using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Api.Options;

public class ImageProcessorOptions
{
    public const string SectionName = "ImageProcessor";

    [Required] public required string ContentType { get; init; }
    [Range(320, 4096)] public int FullWidth { get; init; } = 1024;
}
