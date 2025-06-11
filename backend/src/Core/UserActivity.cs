namespace RateMyPet.Core;

public class UserActivity
{
    public uint Id { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
    public ActivityType Type { get; init; }
    public DateTime Timestamp { get; init; }

    public static UserActivity Register(User user) => new()
    {
        User = user,
        Type = ActivityType.Register
    };

    public static UserActivity ConfirmEmail(User user) => new()
    {
        User = user,
        Type = ActivityType.ConfirmEmail
    };

    public static UserActivity ForgotPassword(Guid userId) => new()
    {
        UserId = userId,
        Type = ActivityType.ForgotPassword
    };

    public static UserActivity ResetPassword(Guid userId) => new()
    {
        UserId = userId,
        Type = ActivityType.ResetPassword
    };
}
