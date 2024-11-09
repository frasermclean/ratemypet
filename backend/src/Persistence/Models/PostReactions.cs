using System.Text.Json.Serialization;

namespace RateMyPet.Persistence.Models;

public class PostReactions
{
    [JsonPropertyName("like")] public uint LikeCount { get; set; }
    [JsonPropertyName("funny")] public uint FunnyCount { get; set; }
    [JsonPropertyName("crazy")] public uint CrazyCount { get; set; }
    [JsonPropertyName("wow")] public uint WowCount { get; set; }
    [JsonPropertyName("sad")] public uint SadCount { get; set; }
}