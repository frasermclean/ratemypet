namespace RateMyPet.Core;

public class Post
{
    public const int TitleMaxLength = 50;
    public const int SlugMaxLength = 60;
    public const int DescriptionMaxLength = 500;

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

    public static string CreateSlug(string title, bool appendRandomSuffix = false, DateOnly? date = null)
    {
        title = title.Trim().ToLowerInvariant().Replace(" ", "-");
        date ??= DateOnly.FromDateTime(DateTime.UtcNow);
        var suffix = appendRandomSuffix ? $"-{Guid.NewGuid():N4}" : "";

        return $"{title}-{date:yyyy-MM-dd}{suffix}";
    }
}
