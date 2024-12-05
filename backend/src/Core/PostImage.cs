namespace RateMyPet.Core;

public class PostImage
{
    public const int BlobNameMaxLength = 40;
    public const int ContentTypeMaxLength = 25;

    public int Width { get; init; }
    public int Height { get; init; }
    public required string BlobName { get; init; }
    public required string ContentType { get; init; }
}
