namespace RateMyPet.Storage;

public static class BlobContainerNames
{
    public const string DataProtection = "data-protection";
    public const string PostImages = "post-images";

    public static string[] All =>
    [
        DataProtection,
        PostImages
    ];
}
