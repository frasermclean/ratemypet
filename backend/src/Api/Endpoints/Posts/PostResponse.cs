using System.Text.Json.Serialization;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Caption { get; init; }
    public required Uri ImageUrl { get; init; }
    public required string AuthorEmailHash { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Reaction? UserReaction { get; init; }

    public required PostReactionsResponse Reactions { get; init; }
}
