using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage.Messaging;

public record PostAddedMessage(Guid PostId) : IMessage;
