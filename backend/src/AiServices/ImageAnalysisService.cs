using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.AiServices;

public class ImageAnalysisService(
    IOptions<AiServicesOptions> options,
    ILogger<ImageAnalysisService> logger,
    ImageAnalysisClient analysisClient) : IImageAnalysisService
{
    private readonly float tagConfidenceThreshold = options.Value.TagConfidenceThreshold;

    public async Task<IEnumerable<string>> GetTagsAsync(BinaryData binaryData,
        CancellationToken cancellationToken = default)
    {
        var response =
            await analysisClient.AnalyzeAsync(binaryData, VisualFeatures.Tags, cancellationToken: cancellationToken);

        // select tags with single word names and high confidence
        var tags = response.Value.Tags.Values
            .Where(tag => !tag.Name.Contains(' ') && tag.Confidence > tagConfidenceThreshold)
            .Select(tag => tag.Name);

        logger.LogInformation("Image analyzed. Tags: {Tags}", tags);

        return tags;
    }
}
