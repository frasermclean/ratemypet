namespace RateMyPet.Core;

public class UserActivity(Guid userId, Activity activity)
{
    public uint Id { get; init; }
    public Guid UserId { get; private init; } = userId;
    public User? User { get; init; }
    public Activity Activity { get; private init; } = activity;
    public DateTime Timestamp { get; init; }
}
