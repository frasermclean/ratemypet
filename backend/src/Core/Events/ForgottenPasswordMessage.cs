namespace RateMyPet.Core.Events;

public class ForgottenPasswordMessage : IMessage
{
    public required string EmailAddress { get; init; }
    public required string ResetCode { get; init; }
}
