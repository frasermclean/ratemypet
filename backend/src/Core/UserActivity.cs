namespace RateMyPet.Core;

public class UserActivity
{
    public long Id { get; init; }
    public required Guid UserId { get; init; }
    public User? User { get; init; }
    public UserActivityCategory Category { get; init; }
    public DateTime Timestamp { get; init; }

    public static UserActivity Register(Guid userId) => new()
    {
        UserId = userId,
        Category = UserActivityCategory.Register
    };

    public static UserActivity ConfirmEmail(Guid userId) => new()
    {
        UserId = userId,
        Category = UserActivityCategory.ConfirmEmail
    };

    public static UserActivity ForgotPassword(Guid userId) => new()
    {
        UserId = userId,
        Category = UserActivityCategory.ForgotPassword
    };

    public static UserActivity ResetPassword(Guid userId) => new()
    {
        UserId = userId,
        Category = UserActivityCategory.ResetPassword
    };
}
