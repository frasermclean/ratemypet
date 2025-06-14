namespace RateMyPet.Core.Abstractions;

public interface IImageAnalysisService
{
    Task<IEnumerable<string>> GetTagsAsync(BinaryData binaryData, CancellationToken cancellationToken = default);
}
