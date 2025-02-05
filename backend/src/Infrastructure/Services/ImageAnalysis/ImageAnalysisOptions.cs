using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.ImageAnalysis;

public class ImageAnalysisOptions
{
    public const string SectionName = "ImageAnalysis";

    [Required] public float TagConfidenceThreshold { get; init; }
    [Required] public int SafetyCategoryThreshold { get; init; }
}
