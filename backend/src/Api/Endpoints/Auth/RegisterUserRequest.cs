namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterUserRequest
{
    public required string Username { get; init; }
    public required string EmailAddress { get;init; }
    public required string Password { get; init; }
}
