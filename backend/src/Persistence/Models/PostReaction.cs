namespace RateMyPet.Persistence.Models;

public class PostReaction
{
    public long Id { get; init; }
    public required User User { get; init; }
    public required Post Post { get; init; }
    public required Reaction Reaction { get; set; }
}
