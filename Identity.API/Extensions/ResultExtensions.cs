using Ardalis.Result;
using Identity.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Extensions;

internal static class ResultExtensions
{
    private static ObjectResult BuildUnauthorizedResponse(IEnumerable<string> errors)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            StatusCode = 401,
            Message = "Unauthorized",
            Errors = errors,
            Data = null
        };

        return new ObjectResult(response) { StatusCode = 401 };
    }

    private static List<string> GetErrors(Result result)
    {
        if (result.Status == ResultStatus.Invalid)
        {
            return result.ValidationErrors.Select(e => e.ErrorMessage).ToList();
        }

        return result.Errors.ToList();
    }

    private static List<string> GetErrors<T>(Result<T> result)
    {
        if (result.Status == ResultStatus.Invalid)
        {
            return result.ValidationErrors.Select(e => e.ErrorMessage).ToList();
        }

        return result.Errors.ToList();
    }

    public static IActionResult ToActionResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkResult(),
            ResultStatus.Created => new CreatedResult(string.Empty, null),
            ResultStatus.NotFound => new NotFoundObjectResult(GetErrors(result)),
            ResultStatus.Conflict => new ConflictObjectResult(GetErrors(result)),
            ResultStatus.Unauthorized => BuildUnauthorizedResponse(GetErrors(result)),
            ResultStatus.Forbidden => new ForbidResult(),
            ResultStatus.Invalid => new BadRequestObjectResult(GetErrors(result)),
            ResultStatus.Error => new ObjectResult(GetErrors(result)) { StatusCode = 500 },
            _ => new ObjectResult(GetErrors(result)) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.IsSuccess,
            StatusCode = result.Status switch
            {
                ResultStatus.Ok => 200,
                ResultStatus.Created => 201,
                ResultStatus.NotFound => 404,
                ResultStatus.Conflict => 409,
                ResultStatus.Unauthorized => 401,
                ResultStatus.Forbidden => 403,
                ResultStatus.Invalid => 400,
                ResultStatus.Error => 500,
                _ => 500
            },
            Message = result.IsSuccess
                ? null
                : result.Status == ResultStatus.Invalid
                    ? "Validation failed"
                    : result.Status == ResultStatus.Unauthorized
                        ? "Unauthorized"
                    : "An error occurred",
            Errors = result.IsSuccess ? null : GetErrors(result),
            Data = result.IsSuccess ? result.Value : default
        };

        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
}
