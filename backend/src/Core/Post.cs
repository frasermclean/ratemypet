namespace RateMyPet.Core;

public class Post
{
    public const int TitleMaxLength = 50;
    public const int SlugMaxLength = 60;
    public const int DescriptionMaxLength = 500;
    public const string ValidTitlePattern = @"^[a-zA-Z0-9\s!?.-]+$";

    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Slug { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required User User { get; init; }
    public required Species Species { get; set; }
    public required PostImage Image { get; init; }
    public ICollection<PostReaction> Reactions { get; } = [];
    public ICollection<PostComment> Comments { get; } = [];
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
