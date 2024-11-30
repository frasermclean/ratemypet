namespace RateMyPet.Api.Endpoints.Auth;

public class LoginRequest
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public bool UseCookies { get; init; }
}
