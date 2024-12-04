namespace RateMyPet.Api.Endpoints.Users;

public class UserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string EmailAddress { get; init; }
    public required string EmailHash { get; init; }
}
