using System.Text.Json.Serialization;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostReactionResponse
{
    [JsonPropertyName("like")] public int LikeCount { get; init; }
    [JsonPropertyName("funny")] public int FunnyCount { get; init; }
    [JsonPropertyName("crazy")] public int CrazyCount { get; init; }
    [JsonPropertyName("wow")] public int WowCount { get; init; }
    [JsonPropertyName("sad")] public int SadCount { get; init; }
}