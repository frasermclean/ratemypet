namespace RateMyPet.Core.Messages;

public record PostAddedMessage(Guid PostId) : IMessage;
