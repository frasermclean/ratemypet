using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class ResetPasswordValidator : Validator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(request => request.ResetCode).NotEmpty();
        RuleFor(request => request.NewPassword).NotEmpty().MinimumLength(8);
    }
}
