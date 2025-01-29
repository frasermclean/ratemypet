namespace RateMyPet.Core;

public class PostImage
{
    public const int AssetIdMaxLength = 64;
    public const int PublicIdMaxLength = 255;

    public string? AssetId { get; set; }
    public string? PublicId { get; set; }
}
