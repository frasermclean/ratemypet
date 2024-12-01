using System.Text.Json.Serialization;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required Uri ImageUrl { get; init; }
    public required string AuthorUserName { get; init; }
    public required string AuthorEmailHash { get; init; }
    public required string SpeciesName { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAtUtc { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Reaction? UserReaction { get; init; }

    public required PostReactionsResponse Reactions { get; init; }
}
