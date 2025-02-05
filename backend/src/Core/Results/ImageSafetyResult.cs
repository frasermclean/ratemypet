namespace RateMyPet.Core.Results;

public class ImageSafetyResult
{
    public bool IsSafe => !IsHate && !IsSelfHarm && !IsSexual && !IsViolence;
    public bool IsHate { get; init; }
    public bool IsSelfHarm { get; init; }
    public bool IsSexual { get; init; }
    public bool IsViolence { get; init; }
}
