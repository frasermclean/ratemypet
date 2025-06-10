namespace RateMyPet.Core;

public class UserActivity
{
    public uint Id { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
    public Activity Activity { get; init; }
    public DateTime Timestamp { get; init; }

    public static UserActivity Register(User user) => new()
    {
        User = user,
        Activity = Activity.Register
    };

    public static UserActivity ConfirmEmail(Guid userId) => new()
    {
        UserId = userId,
        Activity = Activity.ConfirmEmail
    };

    public static UserActivity Login(Guid userId) => new()
    {
        UserId = userId,
        Activity = Activity.Login
    };

    public static UserActivity Logout(Guid userId) => new()
    {
        UserId = userId,
        Activity = Activity.Logout
    };
}
