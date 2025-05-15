using System.ComponentModel.DataAnnotations;

namespace RateMyPet.AiServices;

public class AiServicesOptions
{
    public const string SectionName = "AiServices";

    [Required] public required Uri ComputerVisionEndpoint { get; init; }
    [Required] public required Uri ContentSafetyEndpoint { get; init; }

    [Range(0.1, 0.9)] public float TagConfidenceThreshold { get; init; } = 0.8f;

    [Range(2, 6)] public int HateThreshold { get; init; } = 4;
    [Range(2, 6)] public int SelfHarmThreshold { get; init; } = 4;
    [Range(2, 6)] public int SexualThreshold { get; init; } = 4;
    [Range(2, 6)] public int ViolenceThreshold { get; init; } = 4;
}
