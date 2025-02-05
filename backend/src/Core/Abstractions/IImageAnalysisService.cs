using RateMyPet.Core.Results;

namespace RateMyPet.Core.Abstractions;

public interface IImageAnalysisService
{
    Task<IEnumerable<string>> AnalyzeTagsAsync(Uri imageUri, CancellationToken cancellationToken = default);
    Task<ImageSafetyResult> AnalyzeSafetyAsync(Uri imageUri, CancellationToken cancellationToken = default);
}
