namespace RateMyPet.Core;

public class PostReaction
{
    public long Id { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
    public Guid PostId { get; init; }
    public Post Post { get; init; } = null!;
    public required Reaction Reaction { get; set; }
    public ulong RowVersion { get; init; }
}
