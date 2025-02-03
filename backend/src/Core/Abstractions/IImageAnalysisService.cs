namespace RateMyPet.Core.Abstractions;

public interface IImageAnalysisService
{
    Task<IEnumerable<string>> AnalyzeTagsAsync(Uri imageUri, CancellationToken cancellationToken = default);
}
