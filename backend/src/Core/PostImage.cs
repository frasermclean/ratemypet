namespace RateMyPet.Core;

public class PostImage
{
    public const int AssetIdMaxLength = 64;
    public const int PublicIdMaxLength = 32;

    public string? AssetId { get; init; }
    public string? PublicId { get; init; }
}
