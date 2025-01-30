using FastEndpoints;
using FluentResults;
using FluentValidation.Results;

namespace RateMyPet.Api.Extensions;

public static class ResultExtensions
{
    public static ErrorResponse ToErrorResponse<T>(this Result<T> result, string propertyName,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        var failures = result.Errors
            .Select(error => new ValidationFailure(propertyName, error.Message))
            .ToList();

        return new ErrorResponse(failures, statusCode);
    }
}
