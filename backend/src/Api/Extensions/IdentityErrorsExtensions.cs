using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Api.Extensions;

public static class IdentityErrorsExtensions
{
    public static List<ValidationFailure> ToValidationFailures(this IEnumerable<IdentityError> errors) =>
        errors.Select(error => new ValidationFailure(error.Code, error.Description)).ToList();
}
