using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage.Messaging;

public record PostDeletedMessage(string ImagePublicId, bool? ShouldHardDelete) : IMessage;
