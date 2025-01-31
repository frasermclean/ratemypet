using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Api.Extensions;

public static class IdentityErrorsExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this IEnumerable<IdentityError> errors) =>
        errors.GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

    public static ProblemDetails ToProblemDetails(this IdentityResult result,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        var failures = result.Errors.Select(error => new ValidationFailure(error.Code, error.Description)).ToList();
        return new ProblemDetails(failures, statusCode);
    }
}
