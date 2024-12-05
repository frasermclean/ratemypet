namespace RateMyPet.Api.Endpoints.Auth;

public class GetCurrentUserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string EmailAddress { get; init; }
    public required string EmailHash { get; init; }
    public required IEnumerable<string> Roles { get; init; }
}
