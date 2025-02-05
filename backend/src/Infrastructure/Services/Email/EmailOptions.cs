using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Infrastructure.Services.Email;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required] public required string SenderAddress { get; init; }
    [Required] public required string FrontendBaseUrl { get; init; }
}
