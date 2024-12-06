namespace RateMyPet.Api.Endpoints.Auth;

public class ResetPasswordRequest
{
    public required string EmailAddress { get; init; }
    public required string ResetCode { get; init; }
    public required string NewPassword { get; init; }
}
