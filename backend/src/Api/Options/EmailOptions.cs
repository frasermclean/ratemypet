using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Api.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required] public required string AcsEndpoint { get; init; }
    [Required] public required string SenderAddress { get; init; }
}
