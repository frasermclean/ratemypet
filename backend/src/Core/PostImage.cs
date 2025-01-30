namespace RateMyPet.Core;

public class PostImage
{
    public const int FileNameMaxLength = 256;
    public const int AssetIdMaxLength = 64;
    public const int PublicIdMaxLength = 256;

    public required string FileName { get; init; }
    public required string AssetId { get; init; }
    public required string PublicId { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public long Size { get; init; }
}
