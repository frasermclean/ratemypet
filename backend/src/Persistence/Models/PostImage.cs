namespace RateMyPet.Persistence.Models;

public class PostImage
{
    public const int BlobNameMaxLength = 40;

    public int Width { get; init; }
    public int Height { get; init; }
    public required string BlobName { get; init; }
}
