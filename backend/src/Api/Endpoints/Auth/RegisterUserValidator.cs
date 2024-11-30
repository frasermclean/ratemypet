using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterUserValidator : Validator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(request => request.Username).NotEmpty();
        RuleFor(request => request.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty();
    }
}
