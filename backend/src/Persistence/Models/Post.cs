namespace RateMyPet.Persistence.Models;

public class Post
{
    public const int TitleMaxLength = 50;
    public const int CaptionMaxLength = 80;

    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Caption { get; set; }
    public required User User { get; init; }
    public required Species Species { get; init; }
    public required PostImage Image { get; init; }
    public List<PostReaction> Reactions { get; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong RowVersion { get; init; }
}
