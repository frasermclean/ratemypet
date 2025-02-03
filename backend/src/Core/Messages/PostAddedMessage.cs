namespace RateMyPet.Core.Messages;

public record PostAddedMessage(Guid PostId, string ImagePublicId) : IMessage;
