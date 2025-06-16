namespace RateMyPet.Core;

public class UserActivity
{
    public long Id { get; init; }
    public required Guid UserId { get; init; }
    public User? User { get; init; }
    public required string Code { get; init; }
    public DateTime Timestamp { get; init; }

    public static UserActivity Register(Guid userId) => new()
    {
        UserId = userId,
        Code = "REGI"
    };

    public static UserActivity ConfirmEmail(Guid userId) => new()
    {
        UserId = userId,
        Code = "CNFE"
    };

    public static UserActivity ForgotPassword(Guid userId) => new()
    {
        UserId = userId,
        Code = "FGPW"
    };

    public static UserActivity ResetPassword(Guid userId) => new()
    {
        UserId = userId,
        Code = "RSPW"
    };
}
