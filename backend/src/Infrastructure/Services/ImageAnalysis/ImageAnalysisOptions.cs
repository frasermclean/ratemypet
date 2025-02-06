using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.ImageAnalysis;

public class ImageAnalysisOptions
{
    public const string SectionName = "ImageAnalysis";

    [Range(0.1, 0.9)] public float TagConfidenceThreshold { get; init; } = 0.8f;
}
