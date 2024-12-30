namespace RateMyPet.Core;

public class PostImage
{
    public const int BlobNameMaxLength = 64;

    public string? PreviewBlobName { get; set; }
    public string? FullBlobName { get; set; }
}
