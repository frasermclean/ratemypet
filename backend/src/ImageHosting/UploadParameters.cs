namespace RateMyPet.ImageHosting;

public class UploadParameters
{
    public required string FileName { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Slug { get; init; }
    public required string Environment { get; init; }
    public int SpeciesId { get; init; }
    public Guid UserId { get; init; }
}
