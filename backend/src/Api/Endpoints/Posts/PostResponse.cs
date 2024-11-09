using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Caption { get; init; }
    public required PostReactions Reactions { get; init; }
}