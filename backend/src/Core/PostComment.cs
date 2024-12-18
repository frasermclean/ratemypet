namespace RateMyPet.Core;

public class PostComment
{
    public const int ContentMaxLength = 500;

    public Guid Id { get; init; }
    public required Post Post { get; init; }
    public required User User { get; init; }
    public required string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public PostComment? Parent { get; init; }
    public ulong RowVersion { get; init; }
}
