using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage.Messaging;

public record PostAddedMessage(Guid PostId, string ImageFileName) : IMessage;
