using System.ComponentModel.DataAnnotations;

namespace RateMyPet.ImageHosting;

public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";
    [Required] public required string CloudName { get; init; }
    [Required] public required string ApiKey { get; init; }
    [Required] public required string ApiSecret { get; init; }

    public void Deconstruct(out string cloudName, out string apiKey, out string apiSecret)
    {
        cloudName = CloudName;
        apiKey = ApiKey;
        apiSecret = ApiSecret;
    }
}
