namespace RateMyPet.Core.Messages;

public class PostAddedMessage : IMessage
{
    public required Guid PostId { get; init; }
    public required string ImageBlobName { get; init; }
}
