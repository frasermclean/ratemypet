using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.Moderation;

public class ModerationOptions
{
    public const string SectionName = "Moderation";

    [Range(2, 6)] public int HateThreshold { get; init; } = 4;
    [Range(2, 6)] public int SelfHarmThreshold { get; init; } = 4;
    [Range(2, 6)] public int SexualThreshold { get; init; } = 4;
    [Range(2, 6)] public int ViolenceThreshold { get; init; } = 4;
}
