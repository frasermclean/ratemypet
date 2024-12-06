using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class ForgotPasswordValidator : Validator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(request => request.EmailOrUserName).NotEmpty();
    }
}
