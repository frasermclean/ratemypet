namespace RateMyPet.Api.Endpoints.Auth;

public class LoginRequest
{
    public required string EmailOrPassword { get; init; }
    public required string Password { get; init; }
    public bool UseCookies { get; init; }
}
