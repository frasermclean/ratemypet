using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Api.Options;

public class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required] public required string BaseUrl { get; init; }
}
