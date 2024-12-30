namespace RateMyPet.Core;

public class Post
{
    public const int TitleMaxLength = 50;
    public const int DescriptionMaxLength = 500;

    public Guid Id { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required User User { get; init; }
    public required Species Species { get; set; }
    public PostStatus Status { get; set; }
    public PostImage Image { get; init; } = new();
    public ICollection<PostReaction> Reactions { get; } = [];
    public ICollection<PostComment> Comments { get; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong RowVersion { get; init; }
}
