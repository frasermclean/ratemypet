namespace RateMyPet.Api.Endpoints.Auth;

public class ConfirmEmailRequest
{
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
}
