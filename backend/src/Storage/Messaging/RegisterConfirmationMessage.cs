using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage.Messaging;

public class RegisterConfirmationMessage : IMessage
{
    public required string EmailAddress { get; init; }
    public required Guid UserId { get; init; }
    public required string ConfirmationToken { get; init; }
}
