using FastEndpoints;
using FluentResults;
using FluentValidation.Results;

namespace RateMyPet.Api.Extensions;

public static class ResultExtensions
{
    public static ProblemDetails ToProblemDetails<T>(this Result<T> result, string propertyName,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        var failures = result.Errors
            .Select(error => new ValidationFailure(propertyName, error.Message))
            .ToList();

        return new ProblemDetails(failures, statusCode);
    }
}
