﻿using Azure.AI.ContentSafety;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Results;

namespace RateMyPet.Infrastructure.Services.Moderation;

public class ModerationService(
    ILogger<ModerationService> logger,
    IOptions<ModerationOptions> options,
    ContentSafetyClient safetyClient,
    HttpClient httpClient) : IModerationService
{
    private readonly int hateThreshold = options.Value.HateThreshold;
    private readonly int selfHarmThreshold = options.Value.SelfHarmThreshold;
    private readonly int sexualThreshold = options.Value.SexualThreshold;
    private readonly int violenceThreshold = options.Value.ViolenceThreshold;

    public async Task<ModerationResult> AnalyzeTextAsync(string? text, CancellationToken cancellationToken)
    {
        var response = await safetyClient.AnalyzeTextAsync(text, cancellationToken);

        var categories = response.Value.CategoriesAnalysis
            .ToDictionary(analysis => analysis.Category, analysis => analysis.Severity);

        logger.LogInformation("Text '{Text}' analyzed. Categories: {Categories}", text, categories);

        return new ModerationResult
        {
            IsHate = categories[TextCategory.Hate] >= hateThreshold,
            IsSelfHarm = categories[TextCategory.SelfHarm] >= selfHarmThreshold,
            IsSexual = categories[TextCategory.Sexual] >= sexualThreshold,
            IsViolence = categories[TextCategory.Violence] >= violenceThreshold
        };
    }

    public async Task<ModerationResult> AnalyzeImageAsync(Uri imageUri, CancellationToken cancellationToken)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri, cancellationToken);
        var binaryData = new BinaryData(imageBytes);

        var response = await safetyClient.AnalyzeImageAsync(binaryData, cancellationToken);

        var categories = response.Value.CategoriesAnalysis
            .ToDictionary(analysis => analysis.Category, analysis => analysis.Severity);

        logger.LogInformation("Image {ImageUri} analyzed. Categories: {Categories}", imageUri, categories);

        return new ModerationResult
        {
            IsHate = categories[ImageCategory.Hate] >= hateThreshold,
            IsSelfHarm = categories[ImageCategory.SelfHarm] >= selfHarmThreshold,
            IsSexual = categories[ImageCategory.Sexual] >= sexualThreshold,
            IsViolence = categories[ImageCategory.Violence] >= violenceThreshold
        };
    }
}
