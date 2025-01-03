namespace RateMyPet.Core;

public class PostImage
{
    public const int BlobNameMaxLength = 80;
    public const int FileNameMaxLength = 256;
    public const int MimeTypeMaxLength = 64;

    public required string BlobName { get; set; }
    public required string FileName { get; init; }
    public required string MimeType { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public long Size { get; init; }
    public bool IsProcessed { get; set; }
}
