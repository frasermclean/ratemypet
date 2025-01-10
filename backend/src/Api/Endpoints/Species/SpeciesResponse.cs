namespace RateMyPet.Api.Endpoints.Species;

public class SpeciesResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int PostCount { get; init; }
}
