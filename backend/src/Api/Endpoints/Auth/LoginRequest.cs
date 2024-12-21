namespace RateMyPet.Api.Endpoints.Auth;

public class LoginRequest
{
    public required string EmailOrUserName { get; init; }
    public required string Password { get; init; }
    public bool RememberMe { get; init; }
}
