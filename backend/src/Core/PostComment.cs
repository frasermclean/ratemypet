namespace RateMyPet.Core;

public class PostComment : ISoftDeletable
{
    public const int ContentMaxLength = 500;

    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public Post? Post { get; init; }
    public required Guid UserId { get; init; }
    public User? User { get; init; }
    public required string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? ParentId { get; init; }
    public PostComment? Parent { get; init; }
    public ulong RowVersion { get; init; }
}
