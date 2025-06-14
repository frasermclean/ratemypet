using RateMyPet.Core.Results;

namespace RateMyPet.Core.Abstractions;

public interface IModerationService
{
    Task<ModerationResult> AnalyzeTextAsync(string? text, CancellationToken cancellationToken = default);
    Task<ModerationResult> AnalyzeImageAsync(BinaryData binaryData, CancellationToken cancellationToken = default);
}
