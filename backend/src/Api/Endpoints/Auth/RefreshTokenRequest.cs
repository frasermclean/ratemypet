namespace RateMyPet.Api.Endpoints.Auth;

public class RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
