using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(request => request.EmailOrUserName).NotEmpty();
        RuleFor(request => request.Password).NotEmpty();
    }
}
