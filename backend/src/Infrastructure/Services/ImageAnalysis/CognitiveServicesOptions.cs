using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.ImageAnalysis;

public class CognitiveServicesOptions
{
    public const string SectionName = "CognitiveServices";

    [Required] public required string Endpoint { get; init; }
    public float TagConfidenceThreshold { get; init; } = 0.8f;
}
