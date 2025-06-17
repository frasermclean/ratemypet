namespace RateMyPet.Core;

public class Post : ISoftDeletable
{
    public const int TitleMaxLength = 50;
    public const int SlugMaxLength = 60;
    public const int DescriptionMaxLength = 500;
    public const int TagMinLength = 3;
    public const int TagMaxLength = 20;
    public const int TagsMaxCount = 5;
    public const string ValidTitlePattern = @"^[a-zA-Z0-9\s!?.-]+$";

    public Post(string title, Guid userId, int speciesId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (title.Length > TitleMaxLength)
        {
            throw new ArgumentException($"Title cannot exceed {TitleMaxLength} characters.", nameof(title));
        }

        Title = title.Trim();
        Slug = CreateSlug(Title);
        UserId = userId;
        SpeciesId = speciesId;
        Activities.Add(PostUserActivity.AddPost(userId, this));
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Slug { get; }
    public string Title { get; }
    public string? Description { get; set; }
    public Guid UserId { get; private set; }
    public User? User { get; init; }
    public int SpeciesId { get; set; }
    public Species? Species { get; init; }
    public PostImage? Image { get; set; }
    public ICollection<PostReaction> Reactions { get; } = [];
    public ICollection<PostComment> Comments { get; } = [];
    public ICollection<string> Tags { get; set; } = [];
    public ICollection<PostUserActivity> Activities { get; } = [];
    public PostStatus Status { get; set; } = PostStatus.Initial;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public ulong RowVersion { get; init; }

    public void Update(string description, int speciesId, IEnumerable<string> tags, Guid userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Description = description;
        SpeciesId = speciesId;
        Tags = tags.Distinct().Take(TagsMaxCount).ToList();

        UpdatedAtUtc = DateTime.UtcNow;
        Activities.Add(PostUserActivity.UpdatePost(userId, this));
    }

    internal static string CreateSlug(string title, TimeProvider? timeProvider = null)
    {
        var words = title.ToLowerInvariant()
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
