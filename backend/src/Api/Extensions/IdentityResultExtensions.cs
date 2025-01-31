using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Api.Extensions;

public static class IdentityResultExtensions
{
    public static ProblemDetails ToProblemDetails(this IdentityResult result, string? detail = null,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        var failures = result.Errors
            .Select(error => new ValidationFailure(error.Code, error.Description))
            .ToList();

        return new ProblemDetails(failures, statusCode)
        {
            Detail = detail
        };
    }
}
