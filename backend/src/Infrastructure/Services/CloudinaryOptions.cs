using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services;

public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";
    [Required] public required string CloudName { get; init; }
    [Required] public required string ApiKey { get; init; }
    [Required] public required string ApiSecret { get; init; }
}
