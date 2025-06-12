namespace RateMyPet.Core;

public class Post
{
    public const int TitleMaxLength = 50;
    public const int SlugMaxLength = 60;
    public const int DescriptionMaxLength = 500;
    public const int TagMinLength = 3;
    public const int TagMaxLength = 20;
    public const int TagsMaxCount = 5;
    public const string ValidTitlePattern = @"^[a-zA-Z0-9\s!?.-]+$";

    public Guid Id { get; init; } = Guid.NewGuid();
    public required string? Slug { get; init; }
    public required string Title { get; init; }
    public string? Description { get; set; }
    public required Guid UserId { get; init; }
    public User? User { get; init; }
    public required int SpeciesId { get; init; }
    public Species? Species { get; set; }
    public PostImage? Image { get; set; }
    public ICollection<PostReaction> Reactions { get; } = [];
    public ICollection<PostComment> Comments { get; } = [];
    public ICollection<string> Tags { get; set; } = [];
    public ICollection<PostUserActivity> Activities { get; } = [];
    public PostStatus Status { get; set; } = PostStatus.Initial;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong RowVersion { get; init; }

    public static string CreateSlug(string title, TimeProvider? timeProvider = null)
    {
        var words = title.Trim()
            .ToLowerInvariant()
            .Replace("!", "")
            .Replace("?", "")
            .Replace(".", "")
            .Split(' ');

        var sanitizedTitle = string.Join('-', words.Where(word => word.Length > 0));

        // calculate timestamp from the time provider
        timeProvider ??= TimeProvider.System;
        var timeStamp = timeProvider.GetUtcNow().ToUnixTimeSeconds();

        return $"{sanitizedTitle}-{timeStamp}";
    }
}
