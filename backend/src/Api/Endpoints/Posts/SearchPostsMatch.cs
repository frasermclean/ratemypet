﻿using System.Text.Json.Serialization;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class SearchPostsMatch
{
    public required Guid Id { get; init; }
    public required string? Slug { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public string? ImageId { get; init; }
    public required string AuthorUserName { get; init; }
    public required string AuthorEmailHash { get; init; }
    public required string SpeciesName { get; init; }
    public required ICollection<string> Tags { get; init; }
    public required DateTime CreatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Reaction? UserReaction { get; init; }

    public required PostReactionsResponse Reactions { get; init; }
    public required int ReactionCount { get; init; }
    public required int CommentCount { get; init; }
}
