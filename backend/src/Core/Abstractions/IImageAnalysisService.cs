namespace RateMyPet.Core.Abstractions;

public interface IImageAnalysisService
{
    Task<IEnumerable<string>> GetTagsAsync(Uri imageUri, CancellationToken cancellationToken = default);
}
