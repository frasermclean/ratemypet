using FluentResults;
using FluentValidation.Results;

namespace RateMyPet.Api.Extensions;

public static class ErrorListExtensions
{
    public static IEnumerable<ValidationFailure> ToValidationFailures(this List<IError> errors, string propertyName)
        => errors.Select(error => new ValidationFailure(propertyName, error.Message));
}
