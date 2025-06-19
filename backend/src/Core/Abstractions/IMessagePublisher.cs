namespace RateMyPet.Core.Abstractions;

public interface IMessagePublisher
{
    Task<string> PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
}
