namespace RateMyPet.Api.Endpoints.Auth;

public class VerifyUserResponse
{
    public bool IsAuthenticated { get; init; }
    public LoginResponse? User { get; init; }
}
