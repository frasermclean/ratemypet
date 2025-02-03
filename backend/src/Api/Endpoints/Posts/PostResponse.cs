using System.Text.Json.Serialization;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponse
{
    public required Guid Id { get; init; }
    public required string? Slug { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? ImageId { get; init; }
    public required string AuthorUserName { get; init; }
    public required string AuthorEmailHash { get; init; }
    public required int SpeciesId { get; init; }
    public required IEnumerable<string> Tags { get; init; }
    public required bool IsAnalyzed { get; init; }
    public DateTime CreatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Reaction? UserReaction { get; init; }

    public PostReactionsResponse Reactions { get; init; } = new();
    public IEnumerable<PostCommentResponse> Comments { get; init; } = [];
}
