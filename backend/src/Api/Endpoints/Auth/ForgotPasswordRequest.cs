namespace RateMyPet.Api.Endpoints.Auth;

public class ForgotPasswordRequest
{
    public required string EmailOrUserName { get; init; }
}
