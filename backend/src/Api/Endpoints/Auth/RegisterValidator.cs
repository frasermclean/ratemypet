using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30);

        RuleFor(request => request.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
