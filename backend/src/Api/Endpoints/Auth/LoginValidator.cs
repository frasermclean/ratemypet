using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(request => request.UserName).NotEmpty();
        RuleFor(request => request.Password).NotEmpty();
    }
}
