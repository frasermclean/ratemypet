using System.ComponentModel.DataAnnotations;

namespace RateMyPet.Api.Options;

public class EmailSenderOptions
{
    public const string SectionName = "EmailSender";
    [Required] public required string Endpoint { get; init; }
    [Required] public required string SenderAddress { get; init; }
}
