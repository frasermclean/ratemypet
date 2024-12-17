namespace RateMyPet.Jobs.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    public required string AcsEndpoint { get; init; }
    public required string SenderAddress { get; init; }
    public required string FrontendBaseUrl { get; init; }
}
