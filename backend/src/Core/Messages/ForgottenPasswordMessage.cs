namespace RateMyPet.Core.Messages;

public class ForgottenPasswordMessage : IMessage
{
    public required Guid UserId { get; init; }
    public required string EmailAddress { get; init; }
    public required string ResetCode { get; init; }
}
