﻿namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterRequest
{
    public required string UserName { get; init; }
    public required string EmailAddress { get; init; }
    public required string Password { get; init; }
}
