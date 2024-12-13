namespace RateMyPet.Api.Endpoints.Users;

public class SearchUsersMatch
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string EmailAddress { get; init; }
    public required bool IsEmailConfirmed { get; init; }
    public required DateTime? LastSeen { get; init; }
    public required IEnumerable<string> Roles { get; init; }
}
