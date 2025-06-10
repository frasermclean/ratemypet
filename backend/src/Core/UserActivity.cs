using System.Text.Json.Nodes;

namespace RateMyPet.Core;

public class UserActivity
{
    public uint Id { get; init; }
    public required User User { get; init; }
    public Guid UserId { get; init; }
    public Activity Activity { get; init; }
    public DateTime Timestamp { get; init; }
}
