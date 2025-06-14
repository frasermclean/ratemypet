using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage.Messaging;

public record PostDeletedMessage(Guid PostId) : IMessage;
