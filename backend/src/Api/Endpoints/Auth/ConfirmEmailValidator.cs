using FastEndpoints;
using FluentValidation;

namespace RateMyPet.Api.Endpoints.Auth;

public class ConfirmEmailValidator : Validator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
        RuleFor(request => request.Token).NotEmpty();
    }
}
